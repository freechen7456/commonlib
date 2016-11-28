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

namespace org.apache.commons.cli
{
/**
 * Default parser.
 * 
 * @version $Id: DefaultParser.java 1677454 2015-05-03 17:13:54Z ggregory $
 * @since 1.3
 */
public class DefaultParser : CommandLineParser
{
    /** The command-line instance. */
    protected CommandLine cmd;
    
    /** The current options. */
    protected Options options;

    /**
     * Flag indicating how unrecognized tokens are handled. <tt>true</tt> to stop
     * the parsing and add the remaining tokens to the args list.
     * <tt>false</tt> to throw an exception. 
     */
    protected bool stopAtNonOption;

    /** The token currently processed. */
    protected string currentToken;
 
    /** The last option parsed. */
    protected Option currentOption;
 
    /** Flag indicating if tokens should no longer be analyzed and simply added as arguments of the command line. */
    protected bool skipParsing;
 
    /** The required options and groups expected to be found when parsing the command line. */
    protected List<object> expectedOpts;
 
    public CommandLine parse(Options options, string[] arguments)
    {
        return parse(options, arguments, null);
    }

    /**
     * Parse the arguments according to the specified options and properties.
     *
     * @param options    the specified Options
     * @param arguments  the command line arguments
     * @param properties command line option name-value pairs
     * @return the list of atomic option and value tokens
     *
     * @throws ParseException if there are any problems encountered
     * while parsing the command line tokens.
     */
    public CommandLine parse(Options options, string[] arguments, Dictionary<string, string> properties)
    {
        return parse(options, arguments, properties, false);
    }

    public CommandLine parse(Options options, string[] arguments, bool stopAtNonOption)
    {
        return parse(options, arguments, null, stopAtNonOption);
    }

    /**
     * Parse the arguments according to the specified options and properties.
     *
     * @param options         the specified Options
     * @param arguments       the command line arguments
     * @param properties      command line option name-value pairs
     * @param stopAtNonOption if <tt>true</tt> an unrecognized argument stops
     *     the parsing and the remaining arguments are added to the 
     *     {@link CommandLine}s args list. If <tt>false</tt> an unrecognized
     *     argument triggers a ParseException.
     *
     * @return the list of atomic option and value tokens
     * @throws ParseException if there are any problems encountered
     * while parsing the command line tokens.
     */
    public CommandLine parse(Options options, string[] arguments, Dictionary<string, string> properties, bool stopAtNonOption)
    {
        this.options = options;
        this.stopAtNonOption = stopAtNonOption;
        skipParsing = false;
        currentOption = null;
        expectedOpts = new List<object>(options.getRequiredOptions());

        // clear the data from the groups
        foreach (OptionGroup group in options.getOptionGroups())
        {
            group.setSelected(null);
        }

        cmd = new CommandLine();

        if (arguments != null)
        {
            foreach (string argument in arguments)
            {
                handleToken(argument);
            }
        }

        // check the arguments of the last option
        checkRequiredArgs();

        // add the default options
        handleProperties(properties);

        checkRequiredOptions();

        return cmd;
    }

    /**
     * Sets the values of Options using the values in <code>properties</code>.
     *
     * @param properties The value properties to be processed.
     */
    private void handleProperties(Dictionary<string, string> properties)
    {
        if (properties == null)
        {
            return;
        }

        for (IIterator<string> e = properties.Keys.iterator(); e.hasNext();)
        {
            string option = e.next();

            Option opt = options.getOption(option);
            if (opt == null)
            {
                throw new UnrecognizedOptionException("Default option wasn't defined", option);
            }

            // if the option is part of a group, check if another option of the group has been selected
            OptionGroup group = options.getOptionGroup(opt);
            bool selected = group != null && group.getSelected() != null;

            if (!cmd.hasOption(option) && !selected)
            {
                // get the value from the properties
                string value = properties[option];

                if (opt.hasArg())
                {
                    if (opt.getValues() == null || opt.getValues().Length == 0)
                    {
                        opt.addValueForProcessing(value);
                    }
                }
                else if (!("yes".Equals(value, StringComparison.OrdinalIgnoreCase)
                        || "true".Equals(value, StringComparison.OrdinalIgnoreCase)
                        || "1".Equals(value, StringComparison.OrdinalIgnoreCase)))
                {
                    // if the value is not yes, true or 1 then don't add the option to the CommandLine
                    continue;
                }

                handleOption(opt);
                currentOption = null;
            }
        }
    }

    /**
     * Throws a {@link MissingOptionException} if all of the required options
     * are not present.
     *
     * @throws MissingOptionException if any of the required Options
     * are not present.
     */
    private void checkRequiredOptions()
    {
        // if there are required options that have not been processed
        if (!((IList<object>)expectedOpts).isEmpty())
        {
            throw new MissingOptionException(expectedOpts);
        }
    }

    /**
     * Throw a {@link MissingArgumentException} if the current option
     * didn't receive the number of arguments expected.
     */
    private void checkRequiredArgs()
    {
        if (currentOption != null && currentOption.requiresArg())
        {
            throw new MissingArgumentException(currentOption);
        }
    }

    /**
     * Handle any command line token.
     *
     * @param token the command line token to handle
     * @throws ParseException
     */
    private void handleToken(string token)
    {
        currentToken = token;

        if (skipParsing)
        {
            cmd.addArg(token);
        }
        else if ("--".Equals(token))
        {
            skipParsing = true;
        }
        else if (currentOption != null && currentOption.acceptsArg() && isArgument(token))
        {
            currentOption.addValueForProcessing(Util.stripLeadingAndTrailingQuotes(token));
        }
        else if (token.StartsWith("--"))
        {
            handleLongOption(token);
        }
        else if (token.StartsWith("-") && !"-".Equals(token))
        {
            handleShortAndLongOption(token);
        }
        else
        {
            handleUnknownToken(token);
        }

        if (currentOption != null && !currentOption.acceptsArg())
        {
            currentOption = null;
        }
    }

    /**
     * Returns true is the token is a valid argument.
     *
     * @param token
     */
    private bool isArgument(string token)
    {
        return !isOption(token) || isNegativeNumber(token);
    }

    /**
     * Check if the token is a negative number.
     *
     * @param token
     */
    private bool isNegativeNumber(string token)
    {
        try
        {
            Double.Parse(token);
            return true;
        }
        catch (FormatException e)
        {
            return false;
        }
    }

    /**
     * Tells if the token looks like an option.
     *
     * @param token
     */
    private bool isOption(string token)
    {
        return isLongOption(token) || isShortOption(token);
    }

    /**
     * Tells if the token looks like a short option.
     * 
     * @param token
     */
    private bool isShortOption(string token)
    {
        // short options (-S, -SV, -S=V, -SV1=V2, -S1S2)
        return token.StartsWith("-") && token.Length >= 2 && options.hasShortOption(token.Substring(1, 1));
    }

    /**
     * Tells if the token looks like a long option.
     *
     * @param token
     */
    private bool isLongOption(string token)
    {
        if (!token.StartsWith("-") || token.Length == 1)
        {
            return false;
        }

        int pos = token.IndexOf("=");
        string t = pos == -1 ? token : token.Substring(0, pos);

        if (!options.getMatchingOptions(t).isEmpty())
        {
            // long or partial long options (--L, -L, --L=V, -L=V, --l, --l=V)
            return true;
        }
        else if (getLongPrefix(token) != null && !token.StartsWith("--"))
        {
            // -LV
            return true;
        }

        return false;
    }

    /**
     * Handles an unknown token. If the token starts with a dash an 
     * UnrecognizedOptionException is thrown. Otherwise the token is added 
     * to the arguments of the command line. If the stopAtNonOption flag 
     * is set, this stops the parsing and the remaining tokens are added 
     * as-is in the arguments of the command line.
     *
     * @param token the command line token to handle
     */
    private void handleUnknownToken(string token)
    {
        if (token.StartsWith("-") && token.Length > 1 && !stopAtNonOption)
        {
            throw new UnrecognizedOptionException("Unrecognized option: " + token, token);
        }

        cmd.addArg(token);
        if (stopAtNonOption)
        {
            skipParsing = true;
        }
    }

    /**
     * Handles the following tokens:
     *
     * --L
     * --L=V
     * --L V
     * --l
     *
     * @param token the command line token to handle
     */
    private void handleLongOption(string token)
    {
        if (token.IndexOf('=') == -1)
        {
            handleLongOptionWithoutEqual(token);
        }
        else
        {
            handleLongOptionWithEqual(token);
        }
    }

    /**
     * Handles the following tokens:
     *
     * --L
     * -L
     * --l
     * -l
     * 
     * @param token the command line token to handle
     */
    private void handleLongOptionWithoutEqual(string token)
    {
        IList<string> matchingOpts = options.getMatchingOptions(token);
        if (matchingOpts.isEmpty())
        {
            handleUnknownToken(currentToken);
        }
        else if (matchingOpts.size() > 1)
        {
            throw new AmbiguousOptionException(token, matchingOpts);
        }
        else
        {
            handleOption(options.getOption(matchingOpts[0]));
        }
    }

    /**
     * Handles the following tokens:
     *
     * --L=V
     * -L=V
     * --l=V
     * -l=V
     *
     * @param token the command line token to handle
     */
    private void handleLongOptionWithEqual(string token)
    {
        int pos = token.IndexOf('=');

        string value = token.Substring(pos + 1);

        string opt = token.Substring(0, pos);

        IList<string> matchingOpts = options.getMatchingOptions(opt);
        if (matchingOpts.isEmpty())
        {
            handleUnknownToken(currentToken);
        }
        else if (matchingOpts.size() > 1)
        {
            throw new AmbiguousOptionException(opt, matchingOpts);
        }
        else
        {
            Option option = options.getOption(matchingOpts[0]);

            if (option.acceptsArg())
            {
                handleOption(option);
                currentOption.addValueForProcessing(value);
                currentOption = null;
            }
            else
            {
                handleUnknownToken(currentToken);
            }
        }
    }

    /**
     * Handles the following tokens:
     *
     * -S
     * -SV
     * -S V
     * -S=V
     * -S1S2
     * -S1S2 V
     * -SV1=V2
     *
     * -L
     * -LV
     * -L V
     * -L=V
     * -l
     *
     * @param token the command line token to handle
     */
    private void handleShortAndLongOption(string token)
    {
        string t = Util.stripLeadingHyphens(token);

        int pos = t.IndexOf('=');

        if (t.Length == 1)
        {
            // -S
            if (options.hasShortOption(t))
            {
                handleOption(options.getOption(t));
            }
            else
            {
                handleUnknownToken(token);
            }
        }
        else if (pos == -1)
        {
            // no equal sign found (-xxx)
            if (options.hasShortOption(t))
            {
                handleOption(options.getOption(t));
            }
            else if (!options.getMatchingOptions(t).isEmpty())
            {
                // -L or -l
                handleLongOptionWithoutEqual(token);
            }
            else
            {
                // look for a long prefix (-Xmx512m)
                string opt = getLongPrefix(t);

                if (opt != null && options.getOption(opt).acceptsArg())
                {
                    handleOption(options.getOption(opt));
                    currentOption.addValueForProcessing(t.Substring(opt.Length));
                    currentOption = null;
                }
                else if (isJavaProperty(t))
                {
                    // -SV1 (-Dflag)
                    handleOption(options.getOption(t.Substring(0, 1)));
                    currentOption.addValueForProcessing(t.Substring(1));
                    currentOption = null;
                }
                else
                {
                    // -S1S2S3 or -S1S2V
                    handleConcatenatedOptions(token);
                }
            }
        }
        else
        {
            // equal sign found (-xxx=yyy)
            string opt = t.Substring(0, pos);
            string value = t.Substring(pos + 1);

            if (opt.Length == 1)
            {
                // -S=V
                Option option = options.getOption(opt);
                if (option != null && option.acceptsArg())
                {
                    handleOption(option);
                    currentOption.addValueForProcessing(value);
                    currentOption = null;
                }
                else
                {
                    handleUnknownToken(token);
                }
            }
            else if (isJavaProperty(opt))
            {
                // -SV1=V2 (-Dkey=value)
                handleOption(options.getOption(opt.Substring(0, 1)));
                currentOption.addValueForProcessing(opt.Substring(1));
                currentOption.addValueForProcessing(value);
                currentOption = null;
            }
            else
            {
                // -L=V or -l=V
                handleLongOptionWithEqual(token);
            }
        }
    }

    /**
     * Search for a prefix that is the long name of an option (-Xmx512m)
     *
     * @param token
     */
    private string getLongPrefix(string token)
    {
        string t = Util.stripLeadingHyphens(token);

        int i;
        string opt = null;
        for (i = t.Length - 2; i > 1; i--)
        {
            string prefix = t.Substring(0, i);
            if (options.hasLongOption(prefix))
            {
                opt = prefix;
                break;
            }
        }
        
        return opt;
    }

    /**
     * Check if the specified token is a Java-like property (-Dkey=value).
     */
    private bool isJavaProperty(string token)
    {
        string opt = token.Substring(0, 1);
        Option option = options.getOption(opt);

        return option != null && (option.getArgs() >= 2 || option.getArgs() == Option.UNLIMITED_VALUES);
    }

    private void handleOption(Option option)
    {
        // check the previous option before handling the next one
        checkRequiredArgs();

        option = (Option) option.Clone();

        updateRequiredOptions(option);

        cmd.addOption(option);

        if (option.hasArg())
        {
            currentOption = option;
        }
        else
        {
            currentOption = null;
        }
    }

    /**
     * Removes the option or its group from the list of expected elements.
     *
     * @param option
     */
    private void updateRequiredOptions(Option option)
    {
        if (option.isRequired())
        {
            expectedOpts.Remove(option.getKey());
        }

        // if the option is in an OptionGroup make that option the selected option of the group
        if (options.getOptionGroup(option) != null)
        {
            OptionGroup group = options.getOptionGroup(option);

            if (group.isRequired())
            {
                expectedOpts.Remove(group);
            }

            group.setSelected(option);
        }
    }

    /**
     * Breaks <code>token</code> into its constituent parts
     * using the following algorithm.
     *
     * <ul>
     *  <li>ignore the first character ("<b>-</b>")</li>
     *  <li>foreach remaining character check if an {@link Option}
     *  exists with that id.</li>
     *  <li>if an {@link Option} does exist then add that character
     *  prepended with "<b>-</b>" to the list of processed tokens.</li>
     *  <li>if the {@link Option} can have an argument value and there
     *  are remaining characters in the token then add the remaining
     *  characters as a token to the list of processed tokens.</li>
     *  <li>if an {@link Option} does <b>NOT</b> exist <b>AND</b>
     *  <code>stopAtNonOption</code> <b>IS</b> set then add the special token
     *  "<b>--</b>" followed by the remaining characters and also
     *  the remaining tokens directly to the processed tokens list.</li>
     *  <li>if an {@link Option} does <b>NOT</b> exist <b>AND</b>
     *  <code>stopAtNonOption</code> <b>IS NOT</b> set then add that
     *  character prepended with "<b>-</b>".</li>
     * </ul>
     *
     * @param token The current token to be <b>burst</b>
     * at the first non-Option encountered.
     * @throws ParseException if there are any problems encountered
     *                        while parsing the command line token.
     */
    protected void handleConcatenatedOptions(string token)
    {
        for (int i = 1; i < token.Length; i++)
        {
            string ch = string.Concat(token[i]);

            if (options.hasOption(ch))
            {
                handleOption(options.getOption(ch));

                if (currentOption != null && token.Length != i + 1)
                {
                    // add the trail as an argument of the option
                    currentOption.addValueForProcessing(token.Substring(i + 1));
                    break;
                }
            }
            else
            {
                handleUnknownToken(stopAtNonOption && i > 1 ? token.Substring(i) : token);
                break;
            }
        }
    }
}
}
