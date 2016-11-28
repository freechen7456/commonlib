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
    [TestClass]
    public class OptionsTest
    {
        [TestMethod]
        public void testSimple()
        {
            Options opts = new Options();

            opts.addOption("a", false, "toggle -a");
            opts.addOption("b", true, "toggle -b");

            Assert.IsTrue(opts.hasOption("a"));
            Assert.IsTrue(opts.hasOption("b"));
        }

        [TestMethod]
        public void testDuplicateSimple()
        {
            Options opts = new Options();
            opts.addOption("a", false, "toggle -a");
            opts.addOption("a", true, "toggle -a*");

            Assert.AreEqual("toggle -a*", opts.getOption("a").getDescription(), "last one in wins");
        }

        [TestMethod]
        public void testLong()
        {
            Options opts = new Options();

            opts.addOption("a", "--a", false, "toggle -a");
            opts.addOption("b", "--b", true, "set -b");

            Assert.IsTrue(opts.hasOption("a"));
            Assert.IsTrue(opts.hasOption("b"));
        }

        [TestMethod]
        public void testDuplicateLong()
        {
            Options opts = new Options();
            opts.addOption("a", "--a", false, "toggle -a");
            opts.addOption("a", "--a", false, "toggle -a*");
            Assert.AreEqual("toggle -a*", opts.getOption("a").getDescription(), "last one in wins");
        }

        [TestMethod]
        public void testHelpOptions()
        {
            Option longOnly1 = OptionBuilder.withLongOpt("long-only1").create();
            Option longOnly2 = OptionBuilder.withLongOpt("long-only2").create();
            Option shortOnly1 = OptionBuilder.create("1");
            Option shortOnly2 = OptionBuilder.create("2");
            Option bothA = OptionBuilder.withLongOpt("bothA").create("a");
            Option bothB = OptionBuilder.withLongOpt("bothB").create("b");

            Options options = new Options();
            options.addOption(longOnly1);
            options.addOption(longOnly2);
            options.addOption(shortOnly1);
            options.addOption(shortOnly2);
            options.addOption(bothA);
            options.addOption(bothB);

            List<Option> allOptions = new List<Option>();
            allOptions.Add(longOnly1);
            allOptions.Add(longOnly2);
            allOptions.Add(shortOnly1);
            allOptions.Add(shortOnly2);
            allOptions.Add(bothA);
            allOptions.Add(bothB);

            List<Option> helpOptions = options.helpOptions();

            Assert.IsTrue(helpOptions.containsAll(allOptions), "Everything in all should be in help");
            Assert.IsTrue(allOptions.containsAll(helpOptions), "Everything in help should be in all");
        }

        [TestMethod]
        public void testMissingOptionException()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.isRequired().create("f"));
            try
            {
                new PosixParser().parse(options, new string[0]);
                Assert.Fail("Expected MissingOptionException to be thrown");
            }
            catch (MissingOptionException e)
            {
                Assert.AreEqual("Missing required option: f", e.Message);
            }
        }

        [TestMethod]
        public void testMissingOptionsException()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.isRequired().create("f"));
            options.addOption(OptionBuilder.isRequired().create("x"));
            try
            {
                new PosixParser().parse(options, new string[0]);
                Assert.Fail("Expected MissingOptionException to be thrown");
            }
            catch (MissingOptionException e)
            {
                Assert.AreEqual("Missing required options: f, x", e.Message);
            }
        }

        [TestMethod]
        public void testToString()
        {
            Options options = new Options();
            options.addOption("f", "foo", true, "Foo");
            options.addOption("b", "bar", false, "Bar");

            string s = options.ToString();
            Assert.IsNotNull(s, "null string returned");
            Assert.IsTrue(s.ToLower().Contains("foo"), "foo option missing");
            Assert.IsTrue(s.ToLower().Contains("bar"), "bar option missing");
        }

        [TestMethod]
        public void testGetOptionsGroups()
        {
            Options options = new Options();

            OptionGroup group1 = new OptionGroup();
            group1.addOption(OptionBuilder.create('a'));
            group1.addOption(OptionBuilder.create('b'));

            OptionGroup group2 = new OptionGroup();
            group2.addOption(OptionBuilder.create('x'));
            group2.addOption(OptionBuilder.create('y'));

            options.addOptionGroup(group1);
            options.addOptionGroup(group2);

            Assert.IsNotNull(options.getOptionGroups());
            Assert.AreEqual(2, options.getOptionGroups().Count);
        }

        [TestMethod]
        public void testGetMatchingOpts()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("version").create());
            options.addOption(OptionBuilder.withLongOpt("verbose").create());

            Assert.IsTrue(options.getMatchingOptions("foo").isEmpty());
            Assert.AreEqual(1, options.getMatchingOptions("version").size());
            Assert.AreEqual(2, options.getMatchingOptions("ver").size());
        }
    }
}
