/**
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace org.apache.commons.cli
{
/**
 * Main entry-point into the library.
 * <p>
 * Options represents a collection of {@link Option} objects, which
 * describe the possible options for a command-line.
 * <p>
 * It may flexibly parse long and short options, with or without
 * values.  Additionally, it may parse only a portion of a commandline,
 * allowing for flexible multi-stage parsing.
 *
 * @see org.apache.commons.cli.CommandLine
 *
 * @version $Id: Options.java 1685376 2015-06-14 09:51:59Z britter $
 */
[Serializable]
public class Options
{
    /** The serial version UID. */
    private static readonly long serialVersionUID = 1L;

    /** a map of the options with the character key */
    private readonly Dictionary<string, Option> shortOpts = new Dictionary<string, Option>();

    /** a map of the options with the long key */
    private readonly Dictionary<string, Option> longOpts = new Dictionary<string, Option>();

    /** a map of the required options */
    // N.B. This can contain either a String (addOption) or an OptionGroup (addOptionGroup)
    // TODO this seems wrong
    private readonly List<object> requiredOpts = new List<object>();

    /** a map of the option groups */
    private readonly Dictionary<string, OptionGroup> optionGroups = new Dictionary<string, OptionGroup>();

    /**
     * Add the specified option group.
     *
     * @param group the OptionGroup that is to be added
     * @return the resulting Options instance
     */
    public Options addOptionGroup(OptionGroup group)
    {
        if (group.isRequired())
        {
            requiredOpts.Add(group);
        }

        foreach (Option option in group.getOptions())
        {
            // an Option cannot be required if it is in an
            // OptionGroup, either the group is required or
            // nothing is required
            option.setRequired(false);
            addOption(option);

            optionGroups.Add(option.getKey(), group);
        }

        return this;
    }

    /**
     * Lists the OptionGroups that are members of this Options instance.
     *
     * @return a Collection of OptionGroup instances.
     */
    internal ICollection<OptionGroup> getOptionGroups()
    {
        return new HashSet<OptionGroup>(optionGroups.Values);
    }

    /**
     * Add an option that only contains a short name.
     * The option does not take an argument.
     *
     * @param opt Short single-character name of the option.
     * @param description Self-documenting description
     * @return the resulting Options instance
     * @since 1.3
     */
    public Options addOption(string opt, string description)
    {
        addOption(opt, null, false, description);
        return this;
    }

    /**
     * Add an option that only contains a short-name.
     * It may be specified as requiring an argument.
     *
     * @param opt Short single-character name of the option.
     * @param hasArg flag signally if an argument is required after this option
     * @param description Self-documenting description
     * @return the resulting Options instance
     */
    public Options addOption(string opt, bool hasArg, string description)
    {
        addOption(opt, null, hasArg, description);
        return this;
    }

    /**
     * Add an option that contains a short-name and a long-name.
     * It may be specified as requiring an argument.
     *
     * @param opt Short single-character name of the option.
     * @param longOpt Long multi-character name of the option.
     * @param hasArg flag signally if an argument is required after this option
     * @param description Self-documenting description
     * @return the resulting Options instance
     */
    public Options addOption(string opt, string longOpt, bool hasArg, string description)
    {
        addOption(new Option(opt, longOpt, hasArg, description));
        return this;
    }

    /**
     * Adds an option instance
     *
     * @param opt the option that is to be added
     * @return the resulting Options instance
     */
    public Options addOption(Option opt)
    {
        string key = opt.getKey();

        // add it to the long option list
        if (opt.hasLongOpt())
        {
            longOpts[opt.getLongOpt()] = opt;
        }

        // if the option is required add it to the required list
        if (opt.isRequired())
        {
            if (requiredOpts.Contains(key))
            {
                requiredOpts.Remove(requiredOpts.IndexOf(key));
            }
            requiredOpts.Add(key);
        }

        shortOpts[key] = opt;

        return this;
    }

    /**
     * Retrieve a read-only list of options in this set
     *
     * @return read-only Collection of {@link Option} objects in this descriptor
     */
    public ICollection<Option> getOptions()
    {
        return new List<Option>(helpOptions()).AsReadOnly();
    }

    /**
     * Returns the Options for use by the HelpFormatter.
     *
     * @return the List of Options
     */
    internal List<Option> helpOptions()
    {
        return new List<Option>(shortOpts.Values);
    }

    /**
     * Returns the required options.
     *
     * @return read-only List of required options
     */
    public IList<object> getRequiredOptions()
    {
        return requiredOpts.AsReadOnly();
    }

    /**
     * Retrieve the {@link Option} matching the long or short name specified.
     * The leading hyphens in the name are ignored (up to 2).
     *
     * @param opt short or long name of the {@link Option}
     * @return the option represented by opt
     */
    public Option getOption(string opt)
    {
        opt = Util.stripLeadingHyphens(opt);

        if (shortOpts.ContainsKey(opt))
        {
            return shortOpts[opt];
        }

        return (longOpts.ContainsKey(opt) ? longOpts[opt] : null);
    }

    /**
     * Returns the options with a long name starting with the name specified.
     * 
     * @param opt the partial name of the option
     * @return the options matching the partial name specified, or an empty list if none matches
     * @since 1.3
     */
    public IList<string> getMatchingOptions(string opt)
    {
        opt = Util.stripLeadingHyphens(opt);
        
        List<string> matchingOpts = new List<String>();

        // for a perfect match return the single option only
        if (longOpts.ContainsKey(opt))
        {
            return new List<string>(new string[] {opt}).AsReadOnly();
        }

        foreach (string longOpt in longOpts.Keys)
        {
            if (longOpt.StartsWith(opt))
            {
                matchingOpts.Add(longOpt);
            }
        }
        
        return matchingOpts;
    }

    /**
     * Returns whether the named {@link Option} is a member of this {@link Options}.
     *
     * @param opt short or long name of the {@link Option}
     * @return true if the named {@link Option} is a member of this {@link Options}
     */
    public bool hasOption(string opt)
    {
        opt = Util.stripLeadingHyphens(opt);

        return shortOpts.ContainsKey(opt) || longOpts.ContainsKey(opt);
    }

    /**
     * Returns whether the named {@link Option} is a member of this {@link Options}.
     *
     * @param opt long name of the {@link Option}
     * @return true if the named {@link Option} is a member of this {@link Options}
     * @since 1.3
     */
    public bool hasLongOption(string opt)
    {
        opt = Util.stripLeadingHyphens(opt);

        return longOpts.ContainsKey(opt);
    }

    /**
     * Returns whether the named {@link Option} is a member of this {@link Options}.
     *
     * @param opt short name of the {@link Option}
     * @return true if the named {@link Option} is a member of this {@link Options}
     * @since 1.3
     */
    public bool hasShortOption(string opt)
    {
        opt = Util.stripLeadingHyphens(opt);

        return shortOpts.ContainsKey(opt);
    }

    /**
     * Returns the OptionGroup the <code>opt</code> belongs to.
     * @param opt the option whose OptionGroup is being queried.
     *
     * @return the OptionGroup if <code>opt</code> is part
     * of an OptionGroup, otherwise return null
     */
    public OptionGroup getOptionGroup(Option opt)
    {
        return (optionGroups.ContainsKey(opt.getKey()) ? optionGroups[opt.getKey()] : null);
    }

    /**
     * Dump state, suitable for debugging.
     *
     * @return Stringified form of this object
     */
    public override string ToString()
    {
        StringBuilder buf = new StringBuilder();

        buf.Append("[ Options: [ short ");
        buf.Append(shortOpts.ToDumpString());
        buf.Append(" ] [ long ");
        buf.Append(longOpts.ToDumpString());
        buf.Append(" ]");

        return buf.ToString();
    }
}
}