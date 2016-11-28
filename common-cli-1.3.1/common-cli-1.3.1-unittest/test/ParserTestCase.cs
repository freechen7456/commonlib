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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.apache.commons.cli
{
    /**
     * Abstract test case testing common parser features.
     */
    public abstract class ParserTestCase
    {
        protected CommandLineParser parser;

        protected Options options;

        [TestInitialize]
        public void setUp()
        {
            options = new Options()
                .addOption("a", "enable-a", false, "turn [a] on or off")
                .addOption("b", "bfile", true, "set the value of [b]")
                .addOption("c", "copt", false, "turn [c] on or off");
        }

        [TestMethod]
        public virtual void testSimpleShort()
        {
            string[] args = new string[] { "-a",
                                       "-b", "toast",
                                       "foo", "bar" };

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("a"), "Confirm -a is set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 2, "Confirm size of extra args");
        }

        [TestMethod]
        public virtual void testSimpleLong()
        {
            string[] args = new string[] { "--enable-a",
                                       "--bfile", "toast",
                                       "foo", "bar" };

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("a"), "Confirm -a is set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.IsTrue(cl.getOptionValue("bfile").Equals("toast"), "Confirm arg of --bfile");
            Assert.IsTrue(cl.getArgList().size() == 2, "Confirm size of extra args");
        }

        [TestMethod]
        public virtual void testMultiple()
        {
            string[] args = new string[] { "-c",
                                       "foobar",
                                       "-b", "toast" };

            CommandLine cl = parser.parse(options, args, true);
            Assert.IsTrue(cl.hasOption("c"), "Confirm -c is set");
            Assert.IsTrue(cl.getArgList().size() == 3, "Confirm  3 extra args: " + cl.getArgList().size());

            cl = parser.parse(options, cl.getArgs());

            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is not set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 1, "Confirm  1 extra arg: " + cl.getArgList().Count);
            Assert.IsTrue(cl.getArgList()[0].Equals("foobar"), "Confirm  value of extra arg: " + cl.getArgList()[0]);
        }

        [TestMethod]
        public virtual void testMultipleWithLong()
        {
            string[] args = new string[] { "--copt",
                                       "foobar",
                                       "--bfile", "toast" };

            CommandLine cl = parser.parse(options, args, true);
            Assert.IsTrue(cl.hasOption("c"), "Confirm -c is set");
            Assert.IsTrue(cl.getArgList().size() == 3, "Confirm  3 extra args: " + cl.getArgList().size());

            cl = parser.parse(options, cl.getArgs());

            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is not set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().Count == 1, "Confirm  1 extra arg: " + cl.getArgList().size());
            Assert.IsTrue(cl.getArgList()[0].Equals("foobar"), "Confirm  value of extra arg: " + cl.getArgList()[0]);
        }

        [TestMethod]
        public virtual void testUnrecognizedOption()
        {
            string[] args = new string[] { "-a", "-d", "-b", "toast", "foo", "bar" };

            try
            {
                parser.parse(options, args);
                Assert.Fail("UnrecognizedOptionException wasn't thrown");
            }
            catch (UnrecognizedOptionException e)
            {
                Assert.AreEqual("-d", e.getOption());
            }
        }

        [TestMethod]
        public virtual void testMissingArg()
        {
            string[] args = new string[] { "-b" };

            bool caught = false;

            try
            {
                parser.parse(options, args);
            }
            catch (MissingArgumentException e)
            {
                caught = true;
                Assert.AreEqual("b", e.getOption().getOpt(), "option missing an argument");
            }

            Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
        }

        [TestMethod]
        public virtual void testDoubleDash1()
        {
            string[] args = new string[] { "--copt",
                                       "--",
                                       "-b", "toast" };

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("c"), "Confirm -c is set");
            Assert.IsTrue(!cl.hasOption("b"), "Confirm -b is not set");
            Assert.IsTrue(cl.getArgList().Count == 2, "Confirm 2 extra args: " + cl.getArgList().Count);
        }

        [TestMethod]
        public virtual void testDoubleDash2()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.hasArg().create('n'));
            options.addOption(OptionBuilder.create('m'));

            try
            {
                parser.parse(options, new string[] { "-n", "--", "-m" });
                Assert.Fail("MissingArgumentException not thrown for option -n");
            }
            catch (MissingArgumentException e)
            {
                Assert.IsNotNull(e.getOption(), "option null");
                Assert.AreEqual("n", e.getOption().getOpt());
            }
        }

        [TestMethod]
        public virtual void testSingleDash()
        {
            string[] args = new string[] { "--copt",
                                       "-b", "-",
                                       "-a",
                                       "-" };

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("a"), "Confirm -a is set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("-"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 1, "Confirm 1 extra arg: " + cl.getArgList().size());
            Assert.IsTrue(cl.getArgList()[0].Equals("-"), "Confirm value of extra arg: " + cl.getArgList()[0]);
        }

        [TestMethod]
        public virtual void testStopAtUnexpectedArg()
        {
            string[] args = new string[] { "-c",
                                       "foober",
                                       "-b",
                                       "toast" };

            CommandLine cl = parser.parse(options, args, true);
            Assert.IsTrue(cl.hasOption("c"), "Confirm -c is set");
            Assert.IsTrue(cl.getArgList().size() == 3, "Confirm  3 extra args: " + cl.getArgList().size());
        }

        [TestMethod]
        public virtual void testStopAtExpectedArg()
        {
            string[] args = new string[] { "-b", "foo" };

            CommandLine cl = parser.parse(options, args, true);

            Assert.IsTrue(cl.hasOption('b'), "Confirm -b is set");
            Assert.AreEqual("foo", cl.getOptionValue('b'), "Confirm -b is set");
            Assert.IsTrue(cl.getArgList().size() == 0, "Confirm no extra args: " + cl.getArgList().size());
        }

        [TestMethod]
        public virtual void testStopAtNonOptionShort()
        {
            string[] args = new string[]{"-z",
                                     "-a",
                                     "-btoast"};

            CommandLine cl = parser.parse(options, args, true);
            Assert.IsFalse(cl.hasOption("a"), "Confirm -a is not set");
            Assert.IsTrue(cl.getArgList().size() == 3, "Confirm  3 extra args: " + cl.getArgList().size());
        }

        [TestMethod]
        public virtual void testStopAtNonOptionLong()
        {
            string[] args = new string[]{"--zop==1",
                                     "-abtoast",
                                     "--b=bar"};

            CommandLine cl = parser.parse(options, args, true);

            Assert.IsFalse(cl.hasOption("a"), "Confirm -a is not set");
            Assert.IsFalse(cl.hasOption("b"), "Confirm -b is not set");
            Assert.IsTrue(cl.getArgList().size() == 3, "Confirm  3 extra args: " + cl.getArgList().size());
        }

        [TestMethod]
        public virtual void testNegativeArgument()
        {
            string[] args = new string[] { "-b", "-1" };

            CommandLine cl = parser.parse(options, args);
            Assert.AreEqual("-1", cl.getOptionValue("b"));
        }

        [TestMethod]
        public virtual void testNegativeOption()
        {
            string[] args = new string[] { "-b", "-1" };

            options.addOption("1", false, null);

            CommandLine cl = parser.parse(options, args);
            Assert.AreEqual("-1", cl.getOptionValue("b"));
        }

        [TestMethod]
        public virtual void testArgumentStartingWithHyphen()
        {
            string[] args = new string[] { "-b", "-foo" };

            CommandLine cl = parser.parse(options, args);
            Assert.AreEqual("-foo", cl.getOptionValue("b"));
        }

        [TestMethod]
        public virtual void testShortWithEqual()
        {
            string[] args = new string[] { "-f=bar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasArg().create('f'));

            CommandLine cl = parser.parse(options, args);

            Assert.AreEqual("bar", cl.getOptionValue("foo"));
        }

        [TestMethod]
        public virtual void testShortWithoutEqual()
        {
            string[] args = new string[] { "-fbar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasArg().create('f'));

            CommandLine cl = parser.parse(options, args);

            Assert.AreEqual("bar", cl.getOptionValue("foo"));
        }

        [TestMethod]
        public virtual void testLongWithEqualDoubleDash()
        {
            string[] args = new string[] { "--foo=bar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasArg().create('f'));

            CommandLine cl = parser.parse(options, args);

            Assert.AreEqual("bar", cl.getOptionValue("foo"));
        }

        [TestMethod]
        public virtual void testLongWithEqualSingleDash()
        {
            string[] args = new string[] { "-foo=bar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasArg().create('f'));

            CommandLine cl = parser.parse(options, args);

            Assert.AreEqual("bar", cl.getOptionValue("foo"));
        }

        [TestMethod]
        public virtual void testLongWithoutEqualSingleDash()
        {
            string[] args = new string[] { "-foobar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasArg().create('f'));

            CommandLine cl = parser.parse(options, args);

            Assert.AreEqual("bar", cl.getOptionValue("foo"));
        }

        [TestMethod]
        public virtual void testAmbiguousLongWithoutEqualSingleDash()
        {
            string[] args = new string[] { "-b", "-foobar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasOptionalArg().create('f'));
            options.addOption(OptionBuilder.withLongOpt("bar").hasOptionalArg().create('b'));

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("b"));
            Assert.IsTrue(cl.hasOption("f"));
            Assert.AreEqual("bar", cl.getOptionValue("foo"));
        }

        [TestMethod]
        public virtual void testLongWithoutEqualDoubleDash()
        {
            string[] args = new string[] { "--foobar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").hasArg().create('f'));

            CommandLine cl = parser.parse(options, args, true);

            Assert.IsFalse(cl.hasOption("foo")); // foo isn't expected to be recognized with a double dash
        }

        [TestMethod]
        public virtual void testLongWithUnexpectedArgument1()
        {
            string[] args = new string[] { "--foo=bar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").create('f'));

            try
            {
                parser.parse(options, args);
            }
            catch (UnrecognizedOptionException e)
            {
                Assert.AreEqual("--foo=bar", e.getOption());
                return;
            }

            Assert.Fail("UnrecognizedOptionException not thrown");
        }

        [TestMethod]
        public virtual void testLongWithUnexpectedArgument2()
        {
            string[] args = new string[] { "-foobar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").create('f'));

            try
            {
                parser.parse(options, args);
            }
            catch (UnrecognizedOptionException e)
            {
                Assert.AreEqual("-foobar", e.getOption());
                return;
            }

            Assert.Fail("UnrecognizedOptionException not thrown");
        }

        [TestMethod]
        public virtual void testShortWithUnexpectedArgument()
        {
            string[] args = new string[] { "-f=bar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("foo").create('f'));

            try
            {
                parser.parse(options, args);
            }
            catch (UnrecognizedOptionException e)
            {
                Assert.AreEqual("-f=bar", e.getOption());
                return;
            }

            Assert.Fail("UnrecognizedOptionException not thrown");
        }

        [TestMethod]
        public virtual void testPropertiesOption1()
        {
            string[] args = new string[] { "-Jsource=1.5", "-J", "target", "1.5", "foo" };

            Options options = new Options();
            options.addOption(OptionBuilder.withValueSeparator().hasArgs(2).create('J'));

            CommandLine cl = parser.parse(options, args);

            List<string> values = new List<string>(cl.getOptionValues("J"));
            Assert.IsNotNull(values, "null values");
            Assert.AreEqual(4, values.Count, "number of values");
            Assert.AreEqual("source", values[0], "value 1");
            Assert.AreEqual("1.5", values[1], "value 2");
            Assert.AreEqual("target", values[2], "value 3");
            Assert.AreEqual("1.5", values[3], "value 4");

            List<string> argsleft = cl.getArgList();
            Assert.AreEqual(1, argsleft.Count, "Should be 1 arg left");
            Assert.AreEqual("foo", argsleft[0], "Expecting foo");
        }

        [TestMethod]
        public virtual void testPropertiesOption2()
        {
            string[] args = new string[] { "-Dparam1", "-Dparam2=value2", "-D" };

            Options options = new Options();
            options.addOption(OptionBuilder.withValueSeparator().hasOptionalArgs(2).create('D'));

            CommandLine cl = parser.parse(options, args);

            Dictionary<string, string> props = cl.getOptionProperties("D");
            Assert.IsNotNull(props, "null properties");
            Assert.AreEqual(2, props.Count, "number of properties in " + props);
            Assert.AreEqual("true", props["param1"], "property 1");
            Assert.AreEqual("value2", props["param2"], "property 2");

            List<string> argsleft = cl.getArgList();
            Assert.AreEqual(0, argsleft.size(), "Should be no arg left");
        }

        [TestMethod]
        public virtual void testUnambiguousPartialLongOption1()
        {
            string[] args = new string[] { "--ver" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("help").create());

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("version"), "Confirm --version is set");
        }

        [TestMethod]
        public virtual void testUnambiguousPartialLongOption2()
        {
            string[] args = new string[] { "-ver" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("help").create());

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("version"), "Confirm --version is set");
        }

        [TestMethod]
        public virtual void testUnambiguousPartialLongOption3()
        {
            string[] args = new string[] { "--ver=1" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("verbose").hasOptionalArg().create());
            options.addOption(OptionBuilder.withLongOpt("help").create());

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("verbose"), "Confirm --verbose is set");
            Assert.AreEqual("1", cl.getOptionValue("verbose"));
        }

        [TestMethod]
        public virtual void testUnambiguousPartialLongOption4()
        {
            string[] args = new string[] { "-ver=1" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("verbose").hasOptionalArg().create());
            options.addOption(OptionBuilder.withLongOpt("help").create());

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("verbose"), "Confirm --verbose is set");
            Assert.AreEqual("1", cl.getOptionValue("verbose"));
        }

        [TestMethod]
        public virtual void testAmbiguousPartialLongOption1()
        {
            string[] args = new string[] { "--ver" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("verbose").create());

            bool caught = false;

            try
            {
                parser.parse(options, args);
            }
            catch (AmbiguousOptionException e)
            {
                caught = true;
                Assert.AreEqual("--ver", e.getOption(), "Partial option");
                Assert.IsNotNull(e.getMatchingOptions(), "Matching options null");
                Assert.AreEqual(2, e.getMatchingOptions().Count, "Matching options size");
            }

            Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
        }

        [TestMethod]
        public virtual void testAmbiguousPartialLongOption2()
        {
            string[] args = new string[] { "-ver" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("verbose").create());

            bool caught = false;

            try
            {
                parser.parse(options, args);
            }
            catch (AmbiguousOptionException e)
            {
                caught = true;
                Assert.AreEqual("-ver", e.getOption(), "Partial option");
                Assert.IsNotNull(e.getMatchingOptions(), "Matching options null");
                Assert.AreEqual(2, e.getMatchingOptions().size(), "Matching options size");
            }

            Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
        }

        [TestMethod]
        public virtual void testAmbiguousPartialLongOption3()
        {
            string[] args = new string[] { "--ver=1" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("verbose").hasOptionalArg().create());

            bool caught = false;

            try
            {
                parser.parse(options, args);
            }
            catch (AmbiguousOptionException e)
            {
                caught = true;
                Assert.AreEqual("--ver", e.getOption(), "Partial option");
                Assert.IsNotNull(e.getMatchingOptions(), "Matching options null");
                Assert.AreEqual(2, e.getMatchingOptions().size(), "Matching options size");
            }

            Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
        }

        [TestMethod]
        public virtual void testAmbiguousPartialLongOption4()
        {
            string[] args = new string[] { "-ver=1" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("verbose").hasOptionalArg().create());

            bool caught = false;

            try
            {
                parser.parse(options, args);
            }
            catch (AmbiguousOptionException e)
            {
                caught = true;
                Assert.AreEqual("-ver", e.getOption(), "Partial option");
                Assert.IsNotNull(e.getMatchingOptions(), "Matching options null");
                Assert.AreEqual(2, e.getMatchingOptions().size(), "Matching options size");
            }

            Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
        }

        [TestMethod]
        public virtual void testPartialLongOptionSingleDash()
        {
            string[] args = new string[] { "-ver" };

            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.hasArg().create('v'));

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("version"), "Confirm --version is set");
            Assert.IsTrue(!cl.hasOption("v"), "Confirm -v is not set");
        }

        [TestMethod]
        public virtual void testWithRequiredOption()
        {
            string[] args = new string[] { "-b", "file" };

            Options options = new Options();
            options.addOption("a", "enable-a", false, null);
            options.addOption(OptionBuilder.withLongOpt("bfile").hasArg().isRequired().create('b'));

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(!cl.hasOption("a"), "Confirm -a is NOT set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("file"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 0, "Confirm NO of extra args");
        }

        [TestMethod]
        public virtual void testOptionAndRequiredOption()
        {
            string[] args = new string[] { "-a", "-b", "file" };

            Options options = new Options();
            options.addOption("a", "enable-a", false, null);
            options.addOption(OptionBuilder.withLongOpt("bfile").hasArg().isRequired().create('b'));

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("a"), "Confirm -a is set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("file"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 0, "Confirm NO of extra args");
        }

        [TestMethod]
        public virtual void testMissingRequiredOption()
        {
            string[] args = new string[] { "-a" };

            Options options = new Options();
            options.addOption("a", "enable-a", false, null);
            options.addOption(OptionBuilder.withLongOpt("bfile").hasArg().isRequired().create('b'));

            try
            {
                parser.parse(options, args);
                Assert.Fail("exception should have been thrown");
            }
            catch (MissingOptionException e)
            {
                Assert.AreEqual("Missing required option: b", e.Message, "Incorrect exception message");
                Assert.IsTrue(e.getMissingOptions().Contains("b"));
            }
            catch (ParseException e)
            {
                Assert.Fail("expected to catch MissingOptionException");
            }
        }

        [TestMethod]
        public virtual void testMissingRequiredOptions()
        {
            string[] args = new string[] { "-a" };

            Options options = new Options();
            options.addOption("a", "enable-a", false, null);
            options.addOption(OptionBuilder.withLongOpt("bfile").hasArg().isRequired().create('b'));
            options.addOption(OptionBuilder.withLongOpt("cfile").hasArg().isRequired().create('c'));

            try
            {
                parser.parse(options, args);
                Assert.Fail("exception should have been thrown");
            }
            catch (MissingOptionException e)
            {
                Assert.AreEqual("Missing required options: b, c", e.Message, "Incorrect exception message");
                Assert.IsTrue(e.getMissingOptions().Contains("b"));
                Assert.IsTrue(e.getMissingOptions().Contains("c"));
            }
            catch (ParseException e)
            {
                Assert.Fail("expected to catch MissingOptionException");
            }
        }

        [TestMethod]
        public virtual void testMissingRequiredGroup()
        {
            OptionGroup group = new OptionGroup();
            group.addOption(OptionBuilder.create("a"));
            group.addOption(OptionBuilder.create("b"));
            group.setRequired(true);

            Options options = new Options();
            options.addOptionGroup(group);
            options.addOption(OptionBuilder.isRequired().create("c"));

            try
            {
                parser.parse(options, new string[] { "-c" });
                Assert.Fail("MissingOptionException not thrown");
            }
            catch (MissingOptionException e)
            {
                Assert.AreEqual(1, e.getMissingOptions().Count);
                Assert.IsTrue(e.getMissingOptions()[0] is OptionGroup);
            }
            catch (ParseException e)
            {
                Assert.Fail("Expected to catch MissingOptionException");
            }
        }

        [TestMethod]
        public virtual void testOptionGroup()
        {
            OptionGroup group = new OptionGroup();
            group.addOption(OptionBuilder.create("a"));
            group.addOption(OptionBuilder.create("b"));

            Options options = new Options();
            options.addOptionGroup(group);

            parser.parse(options, new string[] { "-b" });

            Assert.AreEqual("b", group.getSelected(), "selected option");
        }

        [TestMethod]
        public virtual void testOptionGroupLong()
        {
            OptionGroup group = new OptionGroup();
            group.addOption(OptionBuilder.withLongOpt("foo").create());
            group.addOption(OptionBuilder.withLongOpt("bar").create());

            Options options = new Options();
            options.addOptionGroup(group);

            CommandLine cl = parser.parse(options, new string[] { "--bar" });

            Assert.IsTrue(cl.hasOption("bar"));
            Assert.AreEqual("bar", group.getSelected(), "selected option");
        }

        [TestMethod]
        public virtual void testReuseOptionsTwice()
        {
            Options opts = new Options();
            opts.addOption(OptionBuilder.isRequired().create('v'));

            // first parsing
            parser.parse(opts, new string[] { "-v" });

            try
            {
                // second parsing, with the same Options instance and an invalid command line
                parser.parse(opts, new string[0]);
                Assert.Fail("MissingOptionException not thrown");
            }
            catch (MissingOptionException e)
            {
                // expected
            }
        }

        [TestMethod]
        public virtual void testBursting()
        {
            string[] args = new string[] { "-acbtoast", "foo", "bar" };

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("a"), "Confirm -a is set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.hasOption("c"), "Confirm -c is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 2, "Confirm size of extra args");
        }

        [TestMethod]
        public virtual void testUnrecognizedOptionWithBursting()
        {
            string[] args = new string[] { "-adbtoast", "foo", "bar" };

            try
            {
                parser.parse(options, args);
                Assert.Fail("UnrecognizedOptionException wasn't thrown");
            }
            catch (UnrecognizedOptionException e)
            {
                Assert.AreEqual("-adbtoast", e.getOption());
            }
        }

        [TestMethod]
        public virtual void testMissingArgWithBursting()
        {
            string[] args = new string[] { "-acb" };

            bool caught = false;

            try
            {
                parser.parse(options, args);
            }
            catch (MissingArgumentException e)
            {
                caught = true;
                Assert.AreEqual("b", e.getOption().getOpt(), "option missing an argument");
            }

            Assert.IsTrue(caught, "Confirm MissingArgumentException caught");
        }

        [TestMethod]
        public virtual void testStopBursting()
        {
            string[] args = new string[] { "-azc" };

            CommandLine cl = parser.parse(options, args, true);
            Assert.IsTrue(cl.hasOption("a"), "Confirm -a is set");
            Assert.IsFalse(cl.hasOption("c"), "Confirm -c is not set");

            Assert.IsTrue(cl.getArgList().size() == 1, "Confirm  1 extra arg: " + cl.getArgList().size());
            Assert.IsTrue(cl.getArgList().Contains("zc"));
        }

        [TestMethod]
        public virtual void testStopBursting2()
        {
            string[] args = new string[] { "-c", "foobar", "-btoast" };

            CommandLine cl = parser.parse(options, args, true);
            Assert.IsTrue(cl.hasOption("c"), "Confirm -c is set");
            Assert.IsTrue(cl.getArgList().size() == 2, "Confirm  2 extra args: " + cl.getArgList().size());

            cl = parser.parse(options, cl.getArgs());

            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is not set");
            Assert.IsTrue(cl.hasOption("b"), "Confirm -b is set");
            Assert.IsTrue(cl.getOptionValue("b").Equals("toast"), "Confirm arg of -b");
            Assert.IsTrue(cl.getArgList().size() == 1, "Confirm  1 extra arg: " + cl.getArgList().size());
            Assert.IsTrue(cl.getArgList()[0].Equals("foobar"), "Confirm  value of extra arg: " + cl.getArgList()[0]);
        }

        [TestMethod]
        public virtual void testUnlimitedArgs()
        {
            string[] args = new string[] { "-e", "one", "two", "-f", "alpha" };

            Options options = new Options();
            options.addOption(OptionBuilder.hasArgs().create("e"));
            options.addOption(OptionBuilder.hasArgs().create("f"));

            CommandLine cl = parser.parse(options, args);

            Assert.IsTrue(cl.hasOption("e"), "Confirm -e is set");
            Assert.AreEqual(2, cl.getOptionValues("e").Length, "number of arg for -e");
            Assert.IsTrue(cl.hasOption("f"), "Confirm -f is set");
            Assert.AreEqual(1, cl.getOptionValues("f").Length, "number of arg for -f");
        }

        private CommandLine parse(CommandLineParser parser, Options opts, string[] args, Dictionary<string, string> properties)
        {
            if (parser is Parser)
            {
                return ((Parser)parser).parse(opts, args, properties);
            }
            else if (parser is DefaultParser)
            {
                return ((DefaultParser)parser).parse(opts, args, properties);
            }
            else
            {
                throw new UnrecognizedOptionException("Default options not supported by this parser");
            }
        }

        [TestMethod]
        public virtual void testPropertyOptionSingularValue()
        {
            Options opts = new Options();
            opts.addOption(OptionBuilder.hasOptionalArgs(2).withLongOpt("hide").create());

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("hide", "seek");

            CommandLine cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(cmd.hasOption("hide"));
            Assert.AreEqual("seek", cmd.getOptionValue("hide"));
            Assert.IsTrue(!cmd.hasOption("fake"));
        }

        [TestMethod]
        public virtual void testPropertyOptionFlags()
        {
            Options opts = new Options();
            opts.addOption("a", false, "toggle -a");
            opts.addOption("c", "c", false, "toggle -c");
            opts.addOption(OptionBuilder.hasOptionalArg().create('e'));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("a", "true");
            properties.Add("c", "yes");
            properties.Add("e", "1");

            CommandLine cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(cmd.hasOption("a"));
            Assert.IsTrue(cmd.hasOption("c"));
            Assert.IsTrue(cmd.hasOption("e"));


            properties = new Dictionary<string, string>();
            properties.Add("a", "false");
            properties.Add("c", "no");
            properties.Add("e", "0");

            cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(!cmd.hasOption("a"));
            Assert.IsTrue(!cmd.hasOption("c"));
            Assert.IsTrue(cmd.hasOption("e")); // this option accepts an argument


            properties = new Dictionary<string, string>();
            properties.Add("a", "TRUE");
            properties.Add("c", "nO");
            properties.Add("e", "TrUe");

            cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(cmd.hasOption("a"));
            Assert.IsTrue(!cmd.hasOption("c"));
            Assert.IsTrue(cmd.hasOption("e"));


            properties = new Dictionary<string, string>();
            properties.Add("a", "just a string");
            properties.Add("e", "");

            cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(!cmd.hasOption("a"));
            Assert.IsTrue(!cmd.hasOption("c"));
            Assert.IsTrue(cmd.hasOption("e"));


            properties = new Dictionary<string, string>();
            properties.Add("a", "0");
            properties.Add("c", "1");

            cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(!cmd.hasOption("a"));
            Assert.IsTrue(cmd.hasOption("c"));
        }

        [TestMethod]
        public virtual void testPropertyOptionMultipleValues()
        {
            Options opts = new Options();
            opts.addOption(OptionBuilder.hasArgs().withValueSeparator(',').create('k'));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("k", "one,two");

            string[] values = new string[] { "one", "two" };

            CommandLine cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(cmd.hasOption("k"));
            CollectionAssert.AreEqual(values, cmd.getOptionValues('k'));
        }

        [TestMethod]
        public virtual void testPropertyOverrideValues()
        {
            Options opts = new Options();
            opts.addOption(OptionBuilder.hasOptionalArgs(2).create('i'));
            opts.addOption(OptionBuilder.hasOptionalArgs().create('j'));

            string[] args = new string[] { "-j", "found", "-i", "ink" };

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("j", "seek");

            CommandLine cmd = parse(parser, opts, args, properties);
            Assert.IsTrue(cmd.hasOption("j"));
            Assert.AreEqual("found", cmd.getOptionValue("j"));
            Assert.IsTrue(cmd.hasOption("i"));
            Assert.AreEqual("ink", cmd.getOptionValue("i"));
            Assert.IsTrue(!cmd.hasOption("fake"));
        }

        [TestMethod]
        public virtual void testPropertyOptionRequired()
        {
            Options opts = new Options();
            opts.addOption(OptionBuilder.isRequired().create("f"));

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("f", "true");

            CommandLine cmd = parse(parser, opts, null, properties);
            Assert.IsTrue(cmd.hasOption("f"));
        }

        [TestMethod]
        public virtual void testPropertyOptionUnexpected()
        {
            Options opts = new Options();

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("f", "true");

            try
            {
                parse(parser, opts, null, properties);
                Assert.Fail("UnrecognizedOptionException expected");
            }
            catch (UnrecognizedOptionException e)
            {
                // expected
            }
        }

        [TestMethod]
        public virtual void testPropertyOptionGroup()
        {
            Options opts = new Options();

            OptionGroup group1 = new OptionGroup();
            group1.addOption(new Option("a", null));
            group1.addOption(new Option("b", null));
            opts.addOptionGroup(group1);

            OptionGroup group2 = new OptionGroup();
            group2.addOption(new Option("x", null));
            group2.addOption(new Option("y", null));
            opts.addOptionGroup(group2);

            string[] args = new string[] { "-a" };

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("b", "true");
            properties.Add("x", "true");

            CommandLine cmd = parse(parser, opts, args, properties);

            Assert.IsTrue(cmd.hasOption("a"));
            Assert.IsFalse(cmd.hasOption("b"));
            Assert.IsTrue(cmd.hasOption("x"));
            Assert.IsFalse(cmd.hasOption("y"));
        }
    }
}
