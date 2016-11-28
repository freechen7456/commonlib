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
     * OptionBuilder allows the user to create Options using descriptive methods.
     * <p>
     * Details on the Builder pattern can be found at
     * <a href="http://c2.com/cgi-bin/wiki?BuilderPattern">http://c2.com/cgi-bin/wiki?BuilderPattern</a>.
     * <p>
     * This class is NOT thread safe. See <a href="https://issues.apache.org/jira/browse/CLI-209">CLI-209</a>
     * 
     * @version $Id: OptionBuilder.java 1677400 2015-05-03 13:46:08Z britter $
     * @since 1.0
     * @deprecated since 1.3, use {@link Option#builder(String)} instead
     */
    [Obsolete]
    public sealed class OptionBuilder
    {
        ///** long option */
        //private static string longopt;

        ///** option description */
        //private static string description;

        ///** argument name */
        //private static string argName;

        ///** is required? */
        //private static bool required;

        ///** the number of arguments */
        //private static int numberOfArgs = Option.UNINITIALIZED;

        ///** option type */
        //private static Type type;

        ///** option can have an optional argument value */
        //private static bool optionalArg;

        ///** value separator for argument value */
        //private static char valuesep;

        /** option builder instance */
        private static readonly InternalOptionBuilder INSTANCE = new InternalOptionBuilder();

        static OptionBuilder()
        {
            // ensure the consistency of the initial values
            reset();
        }

        /**
         * private constructor to prevent instances being created
         */
        private OptionBuilder()
        {
            // hide the constructor
        }

        /**
         * Resets the member variables to their default values.
         */
        private static void reset()
        {
            //description = null;
            //argName = null;
            //longopt = null;
            //type = typeof(string);
            //required = false;
            //numberOfArgs = Option.UNINITIALIZED;
            //optionalArg = false;
            //valuesep = (char)0;
            INSTANCE.reset();
        }

        /**
         * The next Option created will have the following long option value.
         *
         * @param newLongopt the long option value
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder withLongOpt(string newLongopt)
        {
            INSTANCE.longopt = newLongopt;

            return INSTANCE;
        }

        /**
         * The next Option created will require an argument value.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasArg()
        {
            INSTANCE.numberOfArgs = 1;

            return INSTANCE;
        }

        /**
         * The next Option created will require an argument value if
         * <code>hasArg</code> is true.
         *
         * @param hasArg if true then the Option has an argument value
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasArg(bool hasArg)
        {
            INSTANCE.numberOfArgs = hasArg ? 1 : Option.UNINITIALIZED;

            return INSTANCE;
        }

        /**
         * The next Option created will have the specified argument value name.
         *
         * @param name the name for the argument value
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder withArgName(string name)
        {
            INSTANCE.argName = name;

            return INSTANCE;
        }

        /**
         * The next Option created will be required.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder isRequired()
        {
            INSTANCE.required = true;

            return INSTANCE;
        }

        /**
         * The next Option created uses <code>sep</code> as a means to
         * separate argument values.
         * <p>
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator('=')
         *                           .create('D');
         *
         * String args = "-Dkey=value";
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);  // will be "key"
         * String propertyValue = opt.getValue(1); // will be "value"
         * </pre>
         *
         * @param sep The value separator to be used for the argument values.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder withValueSeparator(char sep)
        {
            INSTANCE.valuesep = sep;

            return INSTANCE;
        }

        /**
         * The next Option created uses '<code>=</code>' as a means to
         * separate argument values.
         *
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator()
         *                           .create('D');
         *
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);
         * String propertyValue = opt.getValue(1);
         * </pre>
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder withValueSeparator()
        {
            INSTANCE.valuesep = '=';

            return INSTANCE;
        }

        /**
         * The next Option created will be required if <code>required</code>
         * is true.
         *
         * @param newRequired if true then the Option is required
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder isRequired(bool newRequired)
        {
            INSTANCE.required = newRequired;

            return INSTANCE;
        }

        /**
         * The next Option created can have unlimited argument values.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasArgs()
        {
            INSTANCE.numberOfArgs = Option.UNLIMITED_VALUES;

            return INSTANCE;
        }

        /**
         * The next Option created can have <code>num</code> argument values.
         *
         * @param num the number of args that the option can have
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasArgs(int num)
        {
            INSTANCE.numberOfArgs = num;

            return INSTANCE;
        }

        /**
         * The next Option can have an optional argument.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasOptionalArg()
        {
            INSTANCE.numberOfArgs = 1;
            INSTANCE.optionalArg = true;

            return INSTANCE;
        }

        /**
         * The next Option can have an unlimited number of optional arguments.
         *
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasOptionalArgs()
        {
            INSTANCE.numberOfArgs = Option.UNLIMITED_VALUES;
            INSTANCE.optionalArg = true;

            return INSTANCE;
        }

        /**
         * The next Option can have the specified number of optional arguments.
         *
         * @param numArgs - the maximum number of optional arguments
         * the next Option created can have.
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder hasOptionalArgs(int numArgs)
        {
            INSTANCE.numberOfArgs = numArgs;
            INSTANCE.optionalArg = true;

            return INSTANCE;
        }

        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         * <p>
         * <b>Note:</b> this method is kept for binary compatibility and the
         * input type is supposed to be a {@link Class} object. 
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @deprecated since 1.3, use {@link #withType(Class)} instead
         */
        [Obsolete]
        public static IOptionBuilder withType(object newType)
        {
            return withType((Type)newType);
        }

        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @since 1.3
         */
        public static IOptionBuilder withType(Type newType)
        {
            INSTANCE.type = newType;

            return INSTANCE;
        }

        /**
         * The next Option created will have the specified description
         *
         * @param newDescription a description of the Option's purpose
         * @return the OptionBuilder instance
         */
        public static IOptionBuilder withDescription(string newDescription)
        {
            INSTANCE.description = newDescription;

            return INSTANCE;
        }

        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the character representation of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public static Option create(char opt)
        {
            return create(string.Concat(opt));
        }

        /**
         * Create an Option using the current settings
         *
         * @return the Option instance
         * @throws IllegalArgumentException if <code>longOpt</code> has not been set.
         */
        public static Option create()
        {
            return INSTANCE.create();
        }

        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the <code>java.lang.String</code> representation
         * of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public static Option create(string opt)
        {
            return INSTANCE.create(opt);
        }
    }

    public interface IOptionBuilder
    {
        /**
         * The next Option created will have the following long option value.
         *
         * @param newLongopt the long option value
         * @return the OptionBuilder instance
         */
        IOptionBuilder withLongOpt(string newLongopt);

        /**
         * The next Option created will require an argument value.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasArg();

        /**
         * The next Option created will require an argument value if
         * <code>hasArg</code> is true.
         *
         * @param hasArg if true then the Option has an argument value
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasArg(bool hasArg);

        /**
         * The next Option created will have the specified argument value name.
         *
         * @param name the name for the argument value
         * @return the OptionBuilder instance
         */
        IOptionBuilder withArgName(string name);

        /**
         * The next Option created will be required.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder isRequired();

        /**
         * The next Option created uses <code>sep</code> as a means to
         * separate argument values.
         * <p>
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator('=')
         *                           .create('D');
         *
         * String args = "-Dkey=value";
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);  // will be "key"
         * String propertyValue = opt.getValue(1); // will be "value"
         * </pre>
         *
         * @param sep The value separator to be used for the argument values.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder withValueSeparator(char sep);

        /**
         * The next Option created uses '<code>=</code>' as a means to
         * separate argument values.
         *
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator()
         *                           .create('D');
         *
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);
         * String propertyValue = opt.getValue(1);
         * </pre>
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder withValueSeparator();

        /**
         * The next Option created will be required if <code>required</code>
         * is true.
         *
         * @param newRequired if true then the Option is required
         * @return the OptionBuilder instance
         */
        IOptionBuilder isRequired(bool newRequired);

        /**
         * The next Option created can have unlimited argument values.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasArgs();

        /**
         * The next Option created can have <code>num</code> argument values.
         *
         * @param num the number of args that the option can have
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasArgs(int num);

        /**
         * The next Option can have an optional argument.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasOptionalArg();

        /**
         * The next Option can have an unlimited number of optional arguments.
         *
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasOptionalArgs();

        /**
         * The next Option can have the specified number of optional arguments.
         *
         * @param numArgs - the maximum number of optional arguments
         * the next Option created can have.
         * @return the OptionBuilder instance
         */
        IOptionBuilder hasOptionalArgs(int numArgs);

        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         * <p>
         * <b>Note:</b> this method is kept for binary compatibility and the
         * input type is supposed to be a {@link Class} object. 
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @deprecated since 1.3, use {@link #withType(Class)} instead
         */
        [Obsolete]
        IOptionBuilder withType(object newType);

        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @since 1.3
         */
        IOptionBuilder withType(Type newType);

        /**
         * The next Option created will have the specified description
         *
         * @param newDescription a description of the Option's purpose
         * @return the OptionBuilder instance
         */
        IOptionBuilder withDescription(string newDescription);

        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the character representation of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        Option create(char opt);

        /**
         * Create an Option using the current settings
         *
         * @return the Option instance
         * @throws IllegalArgumentException if <code>longOpt</code> has not been set.
         */
        Option create();

        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the <code>java.lang.String</code> representation
         * of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        Option create(string opt);
    }

    internal class InternalOptionBuilder : IOptionBuilder
    {
        /** long option */
        internal string longopt;

        /** option description */
        internal string description;

        /** argument name */
        internal string argName;

        /** is required? */
        internal bool required;

        /** the number of arguments */
        internal int numberOfArgs = Option.UNINITIALIZED;

        /** option type */
        internal Type type;

        /** option can have an optional argument value */
        internal bool optionalArg;

        /** value separator for argument value */
        internal char valuesep;

        /**
         * private constructor to prevent instances being created
         */
        internal InternalOptionBuilder()
        {
            // hide the constructor
        }

        /**
         * Resets the member variables to their default values.
         */
        internal void reset()
        {
            description = null;
            argName = null;
            longopt = null;
            type = typeof(string);
            required = false;
            numberOfArgs = Option.UNINITIALIZED;
            optionalArg = false;
            valuesep = (char)0;
        }

        /**
         * The next Option created will have the following long option value.
         *
         * @param newLongopt the long option value
         * @return the OptionBuilder instance
         */
        public IOptionBuilder withLongOpt(string newLongopt)
        {
            this.longopt = newLongopt;

            return this;
        }

        /**
         * The next Option created will require an argument value.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasArg()
        {
            this.numberOfArgs = 1;

            return this;
        }

        /**
         * The next Option created will require an argument value if
         * <code>hasArg</code> is true.
         *
         * @param hasArg if true then the Option has an argument value
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasArg(bool hasArg)
        {
            this.numberOfArgs = hasArg ? 1 : Option.UNINITIALIZED;

            return this;
        }

        /**
         * The next Option created will have the specified argument value name.
         *
         * @param name the name for the argument value
         * @return the OptionBuilder instance
         */
        public IOptionBuilder withArgName(string name)
        {
            this.argName = name;

            return this;
        }

        /**
         * The next Option created will be required.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder isRequired()
        {
            this.required = true;

            return this;
        }

        /**
         * The next Option created uses <code>sep</code> as a means to
         * separate argument values.
         * <p>
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator('=')
         *                           .create('D');
         *
         * String args = "-Dkey=value";
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);  // will be "key"
         * String propertyValue = opt.getValue(1); // will be "value"
         * </pre>
         *
         * @param sep The value separator to be used for the argument values.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder withValueSeparator(char sep)
        {
            this.valuesep = sep;

            return this;
        }

        /**
         * The next Option created uses '<code>=</code>' as a means to
         * separate argument values.
         *
         * <b>Example:</b>
         * <pre>
         * Option opt = OptionBuilder.withValueSeparator()
         *                           .create('D');
         *
         * CommandLine line = parser.parse(args);
         * String propertyName = opt.getValue(0);
         * String propertyValue = opt.getValue(1);
         * </pre>
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder withValueSeparator()
        {
            this.valuesep = '=';

            return this;
        }

        /**
         * The next Option created will be required if <code>required</code>
         * is true.
         *
         * @param newRequired if true then the Option is required
         * @return the OptionBuilder instance
         */
        public IOptionBuilder isRequired(bool newRequired)
        {
            this.required = newRequired;

            return this;
        }

        /**
         * The next Option created can have unlimited argument values.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasArgs()
        {
            this.numberOfArgs = Option.UNLIMITED_VALUES;

            return this;
        }

        /**
         * The next Option created can have <code>num</code> argument values.
         *
         * @param num the number of args that the option can have
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasArgs(int num)
        {
            this.numberOfArgs = num;

            return this;
        }

        /**
         * The next Option can have an optional argument.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasOptionalArg()
        {
            this.numberOfArgs = 1;
            this.optionalArg = true;

            return this;
        }

        /**
         * The next Option can have an unlimited number of optional arguments.
         *
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasOptionalArgs()
        {
            this.numberOfArgs = Option.UNLIMITED_VALUES;
            this.optionalArg = true;

            return this;
        }

        /**
         * The next Option can have the specified number of optional arguments.
         *
         * @param numArgs - the maximum number of optional arguments
         * the next Option created can have.
         * @return the OptionBuilder instance
         */
        public IOptionBuilder hasOptionalArgs(int numArgs)
        {
            this.numberOfArgs = numArgs;
            this.optionalArg = true;

            return this;
        }

        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         * <p>
         * <b>Note:</b> this method is kept for binary compatibility and the
         * input type is supposed to be a {@link Class} object. 
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @deprecated since 1.3, use {@link #withType(Class)} instead
         */
        [Obsolete]
        public IOptionBuilder withType(object newType)
        {
            return withType((Type)newType);
        }

        /**
         * The next Option created will have a value that will be an instance
         * of <code>type</code>.
         *
         * @param newType the type of the Options argument value
         * @return the OptionBuilder instance
         * @since 1.3
         */
        public IOptionBuilder withType(Type newType)
        {
            this.type = newType;

            return this;
        }

        /**
         * The next Option created will have the specified description
         *
         * @param newDescription a description of the Option's purpose
         * @return the OptionBuilder instance
         */
        public IOptionBuilder withDescription(string newDescription)
        {
            this.description = newDescription;

            return this;
        }

        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the character representation of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public Option create(char opt)
        {
            return create(string.Concat(opt));
        }

        /**
         * Create an Option using the current settings
         *
         * @return the Option instance
         * @throws IllegalArgumentException if <code>longOpt</code> has not been set.
         */
        public Option create()
        {
            if (longopt == null)
            {
                reset();
                throw new ArgumentException("must specify longopt");
            }

            return create(null);
        }

        /**
         * Create an Option using the current settings and with
         * the specified Option <code>char</code>.
         *
         * @param opt the <code>java.lang.String</code> representation
         * of the Option
         * @return the Option instance
         * @throws IllegalArgumentException if <code>opt</code> is not
         * a valid character.  See Option.
         */
        public Option create(string opt)
        {
            Option option = null;
            try
            {
                // create the option
                option = new Option(opt, description);

                // set the option properties
                option.setLongOpt(longopt);
                option.setRequired(required);
                option.setOptionalArg(optionalArg);
                option.setArgs(numberOfArgs);
                option.setType(type);
                option.setValueSeparator(valuesep);
                option.setArgName(argName);
            }
            finally
            {
                // reset the OptionBuilder properties
                reset();
            }

            // return the Option instance
            return option;
        }
    }
}
