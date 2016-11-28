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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.apache.commons.cli
{
    [TestClass]
    public class ValueTest
    {
        private CommandLine _cl = null;
        private readonly Options opts = new Options();

        [TestInitialize]
        public void setUp()
        {
            opts.addOption("a", false, "toggle -a");
            opts.addOption("b", true, "set -b");
            opts.addOption("c", "c", false, "toggle -c");
            opts.addOption("d", "d", true, "set -d");

            opts.addOption(OptionBuilder.hasOptionalArg().create('e'));
            opts.addOption(OptionBuilder.hasOptionalArg().withLongOpt("fish").create());
            opts.addOption(OptionBuilder.hasOptionalArgs().withLongOpt("gravy").create());
            opts.addOption(OptionBuilder.hasOptionalArgs(2).withLongOpt("hide").create());
            opts.addOption(OptionBuilder.hasOptionalArgs(2).create('i'));
            opts.addOption(OptionBuilder.hasOptionalArgs().create('j'));

            string[] args = new string[] { "-a",
            "-b", "foo",
            "--c",
            "--d", "bar" 
        };

            Parser parser = new PosixParser();
            _cl = parser.parse(opts, args);
        }

        [TestMethod]
        public void testShortNoArg()
        {
            Assert.IsTrue(_cl.hasOption("a"));
            Assert.IsNull(_cl.getOptionValue("a"));
        }

        [TestMethod]
        public void testShortWithArg()
        {
            Assert.IsTrue(_cl.hasOption("b"));
            Assert.IsNotNull(_cl.getOptionValue("b"));
            Assert.AreEqual(_cl.getOptionValue("b"), "foo");
        }

        [TestMethod]
        public void testLongNoArg()
        {
            Assert.IsTrue(_cl.hasOption("c"));
            Assert.IsNull(_cl.getOptionValue("c"));
        }

        [TestMethod]
        public void testLongWithArg()
        {
            Assert.IsTrue(_cl.hasOption("d"));
            Assert.IsNotNull(_cl.getOptionValue("d"));
            Assert.AreEqual(_cl.getOptionValue("d"), "bar");
        }

        [TestMethod]
        public void testShortOptionalArgNoValue()
        {
            string[] args = new string[] { "-e" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("e"));
            Assert.IsNull(cmd.getOptionValue("e"));
        }

        [TestMethod]
        public void testShortOptionalArgValue()
        {
            string[] args = new string[] { "-e", "everything" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("e"));
            Assert.AreEqual("everything", cmd.getOptionValue("e"));
        }

        [TestMethod]
        public void testLongOptionalNoValue()
        {
            string[] args = new string[] { "--fish" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("fish"));
            Assert.IsNull(cmd.getOptionValue("fish"));
        }

        [TestMethod]
        public void testLongOptionalArgValue()
        {
            string[] args = new string[] { "--fish", "face" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("fish"));
            Assert.AreEqual("face", cmd.getOptionValue("fish"));
        }

        [TestMethod]
        public void testShortOptionalArgValues()
        {
            string[] args = new string[] { "-j", "ink", "idea" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("j"));
            Assert.AreEqual("ink", cmd.getOptionValue("j"));
            Assert.AreEqual("ink", cmd.getOptionValues("j")[0]);
            Assert.AreEqual("idea", cmd.getOptionValues("j")[1]);
            Assert.AreEqual(cmd.getArgs().Length, 0);
        }

        [TestMethod]
        public void testLongOptionalArgValues()
        {
            string[] args = new string[] { "--gravy", "gold", "garden" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("gravy"));
            Assert.AreEqual("gold", cmd.getOptionValue("gravy"));
            Assert.AreEqual("gold", cmd.getOptionValues("gravy")[0]);
            Assert.AreEqual("garden", cmd.getOptionValues("gravy")[1]);
            Assert.AreEqual(cmd.getArgs().Length, 0);
        }

        [TestMethod]
        public void testShortOptionalNArgValues()
        {
            string[] args = new string[] { "-i", "ink", "idea", "isotope", "ice" };

            Parser parser = new PosixParser();
            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("i"));
            Assert.AreEqual("ink", cmd.getOptionValue("i"));
            Assert.AreEqual("ink", cmd.getOptionValues("i")[0]);
            Assert.AreEqual("idea", cmd.getOptionValues("i")[1]);
            Assert.AreEqual(cmd.getArgs().Length, 2);
            Assert.AreEqual("isotope", cmd.getArgs()[0]);
            Assert.AreEqual("ice", cmd.getArgs()[1]);
        }

        [TestMethod]
        public void testLongOptionalNArgValues()
        {
            string[] args = new string[] { 
            "--hide", "house", "hair", "head"
        };

            Parser parser = new PosixParser();

            CommandLine cmd = parser.parse(opts, args);
            Assert.IsTrue(cmd.hasOption("hide"));
            Assert.AreEqual("house", cmd.getOptionValue("hide"));
            Assert.AreEqual("house", cmd.getOptionValues("hide")[0]);
            Assert.AreEqual("hair", cmd.getOptionValues("hide")[1]);
            Assert.AreEqual(cmd.getArgs().Length, 1);
            Assert.AreEqual("head", cmd.getArgs()[0]);
        }
    }
}
