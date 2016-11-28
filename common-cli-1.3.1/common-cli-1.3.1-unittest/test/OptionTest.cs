/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.apache.commons.cli
{
    [TestClass]
    public class OptionTest
    {
        private class TestOption : Option
        {
            private static readonly long serialVersionUID = 1L;

            public TestOption(string opt, bool hasArg, string description)
                : base(opt, hasArg, description)
            {
            }

            public override bool addValue(string value)
            {
                addValueForProcessing(value);
                return true;
            }
        }

        [TestMethod]
        public void testClear()
        {
            TestOption option = new TestOption("x", true, "");
            Assert.AreEqual(0, option.getValuesList().size());
            option.addValue("a");
            Assert.AreEqual(1, option.getValuesList().size());
            option.clearValues();
            Assert.AreEqual(0, option.getValuesList().size());
        }

        // See http://issues.apache.org/jira/browse/CLI-21
        [TestMethod]
        public void testClone()
        {
            TestOption a = new TestOption("a", true, "");
            TestOption b = (TestOption)a.Clone();
            Assert.AreEqual(a, b);
            Assert.AreNotSame(a, b);
            a.setDescription("a");
            Assert.AreEqual("", b.getDescription());
            b.setArgs(2);
            b.addValue("b1");
            b.addValue("b2");
            Assert.AreEqual(1, a.getArgs());
            Assert.AreEqual(0, a.getValuesList().size());
            Assert.AreEqual(2, b.getValues().Length);
        }

        private class DefaultOption : Option
        {
            private static readonly long serialVersionUID = 1L;

            private readonly string defaultValue;

            public DefaultOption(string opt, string description, string defaultValue)
                : base(opt, true, description)
            {
                this.defaultValue = defaultValue;
            }

            public override string getValue()
            {
                return base.getValue() != null ? base.getValue() : defaultValue;
            }
        }

        [TestMethod]
        public void testSubclass()
        {
            Option option = new DefaultOption("f", "file", "myfile.txt");
            Option clone = (Option)option.Clone();
            Assert.AreEqual("myfile.txt", clone.getValue());
            Assert.AreEqual(typeof(DefaultOption), clone.GetType());
        }

        [TestMethod]
        public void testHasArgName()
        {
            Option option = new Option("f", null);

            option.setArgName(null);
            Assert.IsFalse(option.hasArgName());

            option.setArgName("");
            Assert.IsFalse(option.hasArgName());

            option.setArgName("file");
            Assert.IsTrue(option.hasArgName());
        }

        [TestMethod]
        public void testHasArgs()
        {
            Option option = new Option("f", null);

            option.setArgs(0);
            Assert.IsFalse(option.hasArgs());

            option.setArgs(1);
            Assert.IsFalse(option.hasArgs());

            option.setArgs(10);
            Assert.IsTrue(option.hasArgs());

            option.setArgs(Option.UNLIMITED_VALUES);
            Assert.IsTrue(option.hasArgs());

            option.setArgs(Option.UNINITIALIZED);
            Assert.IsFalse(option.hasArgs());
        }

        [TestMethod]
        public void testGetValue()
        {
            Option option = new Option("f", null);
            option.setArgs(Option.UNLIMITED_VALUES);

            Assert.AreEqual("default", option.getValue("default"));
            Assert.AreEqual(null, option.getValue(0));

            option.addValueForProcessing("foo");

            Assert.AreEqual("foo", option.getValue());
            Assert.AreEqual("foo", option.getValue(0));
            Assert.AreEqual("foo", option.getValue("default"));
        }

        [TestMethod]
        public void testBuilderMethods()
        {
            char defaultSeparator = (char)0;

            checkOption(Option.builder("a").desc("desc").build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").longOpt("aaa").build(),
                "a", "desc", "aaa", Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").hasArg(true).build(),
                "a", "desc", null, 1, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").hasArg(false).build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").hasArg(true).build(),
                "a", "desc", null, 1, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").numberOfArgs(3).build(),
                "a", "desc", null, 3, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").required(true).build(),
                "a", "desc", null, Option.UNINITIALIZED, null, true, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").required(false).build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));

            checkOption(Option.builder("a").desc("desc").argName("arg1").build(),
                "a", "desc", null, Option.UNINITIALIZED, "arg1", false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").optionalArg(false).build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").optionalArg(true).build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, true, defaultSeparator, typeof(string));
            checkOption(Option.builder("a").desc("desc").valueSeparator(':').build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, ':', typeof(string));
            checkOption(Option.builder("a").desc("desc").type(typeof(int)).build(),
                "a", "desc", null, Option.UNINITIALIZED, null, false, false, defaultSeparator, typeof(int));
        }

        [TestMethod]
        public void testBuilderInsufficientParams1()
        {
            try
            {
                Option.builder().desc("desc").build();
                Assert.Fail();
            }
            catch (ArgumentException e) { }
        }

        [TestMethod]
        public void testBuilderInsufficientParams2()
        {
            try
            {
                Option.builder(null).desc("desc").build();
                Assert.Fail();
            }
            catch (ArgumentException e) { }
        }

        private static void checkOption(Option option, string opt, string description, string longOpt, int numArgs,
                                        string argName, bool required, bool optionalArg,
                                        char valueSeparator, Type cls)
        {
            Assert.AreEqual(opt, option.getOpt());
            Assert.AreEqual(description, option.getDescription());
            Assert.AreEqual(longOpt, option.getLongOpt());
            Assert.AreEqual(numArgs, option.getArgs());
            Assert.AreEqual(argName, option.getArgName());
            Assert.AreEqual(required, option.isRequired());

            Assert.AreEqual(optionalArg, option.hasOptionalArg());
            Assert.AreEqual(valueSeparator, option.getValueSeparator());
            Assert.AreEqual(cls, option.getType());
        }

    }
}
