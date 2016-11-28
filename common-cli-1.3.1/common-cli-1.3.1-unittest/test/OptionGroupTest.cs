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
    public class OptionGroupTest
    {
        private Options _options = null;
        private readonly Parser parser = new PosixParser();

        [TestInitialize]
        public void setUp()
        {
            Option file = new Option("f", "file", false, "file to process");
            Option dir = new Option("d", "directory", false, "directory to process");
            OptionGroup group = new OptionGroup();
            group.addOption(file);
            group.addOption(dir);
            _options = new Options().addOptionGroup(group);

            Option section = new Option("s", "section", false, "section to process");
            Option chapter = new Option("c", "chapter", false, "chapter to process");
            OptionGroup group2 = new OptionGroup();
            group2.addOption(section);
            group2.addOption(chapter);

            _options.addOptionGroup(group2);

            Option importOpt = new Option(null, "import", false, "section to process");
            Option exportOpt = new Option(null, "export", false, "chapter to process");
            OptionGroup group3 = new OptionGroup();
            group3.addOption(importOpt);
            group3.addOption(exportOpt);
            _options.addOptionGroup(group3);

            _options.addOption("r", "revision", false, "revision number");
        }

        [TestMethod]
        public void testSingleOptionFromGroup()
        {
            string[] args = new string[] { "-f" };

            CommandLine cl = parser.parse(_options, args);

            Assert.IsTrue(!cl.hasOption("r"), "Confirm -r is NOT set");
            Assert.IsTrue(cl.hasOption("f"), "Confirm -f is set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(!cl.hasOption("s"), "Confirm -s is NOT set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 0, "Confirm no extra args");
        }

        [TestMethod]
        public void testSingleOption()
        {
            string[] args = new string[] { "-r" };

            CommandLine cl = parser.parse(_options, args);

            Assert.IsTrue(cl.hasOption("r"), "Confirm -r is set");
            Assert.IsTrue(!cl.hasOption("f"), "Confirm -f is NOT set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(!cl.hasOption("s"), "Confirm -s is NOT set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 0, "Confirm no extra args");
        }

        [TestMethod]
        public void testTwoValidOptions()
        {
            string[] args = new string[] { "-r", "-f" };

            CommandLine cl = parser.parse(_options, args);

            Assert.IsTrue(cl.hasOption("r"), "Confirm -r is set");
            Assert.IsTrue(cl.hasOption("f"), "Confirm -f is set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(!cl.hasOption("s"), "Confirm -s is NOT set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 0, "Confirm no extra args");
        }

        [TestMethod]
        public void testSingleLongOption()
        {
            string[] args = new string[] { "--file" };

            CommandLine cl = parser.parse(_options, args);

            Assert.IsTrue(!cl.hasOption("r"), "Confirm -r is NOT set");
            Assert.IsTrue(cl.hasOption("f"), "Confirm -f is set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(!cl.hasOption("s"), "Confirm -s is NOT set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 0, "Confirm no extra args");
        }

        [TestMethod]
        public void testTwoValidLongOptions()
        {
            string[] args = new string[] { "--revision", "--file" };

            CommandLine cl = parser.parse(_options, args);

            Assert.IsTrue(cl.hasOption("r"), "Confirm -r is set");
            Assert.IsTrue(cl.hasOption("f"), "Confirm -f is set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(!cl.hasOption("s"), "Confirm -s is NOT set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 0, "Confirm no extra args");
        }

        [TestMethod]
        public void testNoOptionsExtraArgs()
        {
            string[] args = new string[] { "arg1", "arg2" };

            CommandLine cl = parser.parse(_options, args);

            Assert.IsTrue(!cl.hasOption("r"), "Confirm -r is NOT set");
            Assert.IsTrue(!cl.hasOption("f"), "Confirm -f is NOT set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(!cl.hasOption("s"), "Confirm -s is NOT set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 2, "Confirm TWO extra args");
        }

        [TestMethod]
        public void testTwoOptionsFromGroup()
        {
            string[] args = new string[] { "-f", "-d" };

            try
            {
                parser.parse(_options, args);
                Assert.Fail("two arguments from group not allowed");
            }
            catch (AlreadySelectedException e)
            {
                Assert.IsNotNull(e.getOptionGroup(), "null option group");
                Assert.AreEqual("f", e.getOptionGroup().getSelected(), "selected option");
                Assert.AreEqual("d", e.getOption().getOpt(), "option");
            }
        }

        [TestMethod]
        public void testTwoLongOptionsFromGroup()
        {
            string[] args = new string[] { "--file", "--directory" };

            try
            {
                parser.parse(_options, args);
                Assert.Fail("two arguments from group not allowed");
            }
            catch (AlreadySelectedException e)
            {
                Assert.IsNotNull(e.getOptionGroup(), "null option group");
                Assert.AreEqual("f", e.getOptionGroup().getSelected(), "selected option");
                Assert.AreEqual("d", e.getOption().getOpt(), "option");
            }
        }

        [TestMethod]
        public void testTwoOptionsFromDifferentGroup()
        {
            string[] args = new string[] { "-f", "-s" };

            CommandLine cl = parser.parse(_options, args);
            Assert.IsTrue(!cl.hasOption("r"), "Confirm -r is NOT set");
            Assert.IsTrue(cl.hasOption("f"), "Confirm -f is set");
            Assert.IsTrue(!cl.hasOption("d"), "Confirm -d is NOT set");
            Assert.IsTrue(cl.hasOption("s"), "Confirm -s is set");
            Assert.IsTrue(!cl.hasOption("c"), "Confirm -c is NOT set");
            Assert.IsTrue(cl.getArgList().Count == 0, "Confirm NO extra args");
        }

        [TestMethod]
        public void testTwoOptionsFromGroupWithProperties()
        {
            string[] args = new string[] { "-f" };

            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add("d", "true");

            CommandLine cl = parser.parse(_options, args, properties);
            Assert.IsTrue(cl.hasOption("f"));
            Assert.IsTrue(!cl.hasOption("d"));
        }

        [TestMethod]
        public void testValidLongOnlyOptions()
        {
            CommandLine cl1 = parser.parse(_options, new string[] { "--export" });
            Assert.IsTrue(cl1.hasOption("export"), "Confirm --export is set");

            CommandLine cl2 = parser.parse(_options, new string[] { "--import" });
            Assert.IsTrue(cl2.hasOption("import"), "Confirm --import is set");
        }

        [TestMethod]
        public void testToString()
        {
            OptionGroup group1 = new OptionGroup();
            group1.addOption(new Option(null, "foo", false, "Foo"));
            group1.addOption(new Option(null, "bar", false, "Bar"));

            if (!"[--bar Bar, --foo Foo]".Equals(group1.ToString()))
            {
                Assert.AreEqual("[--foo Foo, --bar Bar]", group1.ToString());
            }

            OptionGroup group2 = new OptionGroup();
            group2.addOption(new Option("f", "foo", false, "Foo"));
            group2.addOption(new Option("b", "bar", false, "Bar"));

            if (!"[-b Bar, -f Foo]".Equals(group2.ToString()))
            {
                Assert.AreEqual("[-f Foo, -b Bar]", group2.ToString());
            }
        }

        [TestMethod]
        public void testGetNames()
        {
            OptionGroup group = new OptionGroup();
            group.addOption(OptionBuilder.create('a'));
            group.addOption(OptionBuilder.create('b'));

            Assert.IsNotNull(group.getNames(), "null names");
            Assert.AreEqual(2, group.getNames().Count);
            Assert.IsTrue(group.getNames().Contains("a"));
            Assert.IsTrue(group.getNames().Contains("b"));
        }
    }
}
