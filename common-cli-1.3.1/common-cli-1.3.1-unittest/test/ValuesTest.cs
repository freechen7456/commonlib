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
    public class ValuesTest
    {
        private CommandLine cmd;

        [TestInitialize]
        public void setUp()
        {
            Options options = new Options();

            options.addOption("a", false, "toggle -a");
            options.addOption("b", true, "set -b");
            options.addOption("c", "c", false, "toggle -c");
            options.addOption("d", "d", true, "set -d");

            options.addOption(OptionBuilder.withLongOpt("e").hasArgs().withDescription("set -e ").create('e'));
            options.addOption("f", "f", false, "jk");
            options.addOption(OptionBuilder.withLongOpt("g").hasArgs(2).withDescription("set -g").create('g'));
            options.addOption(OptionBuilder.withLongOpt("h").hasArg().withDescription("set -h").create('h'));
            options.addOption(OptionBuilder.withLongOpt("i").withDescription("set -i").create('i'));
            options.addOption(OptionBuilder.withLongOpt("j").hasArgs().withDescription("set -j").withValueSeparator('=').create('j'));
            options.addOption(OptionBuilder.withLongOpt("k").hasArgs().withDescription("set -k").withValueSeparator('=').create('k'));
            options.addOption(OptionBuilder.withLongOpt("m").hasArgs().withDescription("set -m").withValueSeparator().create('m'));

            string[] args = new string[] { "-a",
                                       "-b", "foo",
                                       "--c",
                                       "--d", "bar",
                                       "-e", "one", "two",
                                       "-f",
                                       "arg1", "arg2",
                                       "-g", "val1", "val2" , "arg3",
                                       "-h", "val1", "-i",
                                       "-h", "val2",
                                       "-jkey=value",
                                       "-j", "key=value",
                                       "-kkey1=value1", 
                                       "-kkey2=value2",
                                       "-mkey=value"};

            CommandLineParser parser = new PosixParser();

            cmd = parser.parse(options, args);
        }

        [TestMethod]
        public void testShortArgs()
        {
            Assert.IsTrue(cmd.hasOption("a"), "Option a is not set");
            Assert.IsTrue(cmd.hasOption("c"), "Option c is not set");

            Assert.IsNull(cmd.getOptionValues("a"));
            Assert.IsNull(cmd.getOptionValues("c"));
        }

        [TestMethod]
        public void testShortArgsWithValue()
        {
            Assert.IsTrue(cmd.hasOption("b"), "Option b is not set");
            Assert.IsTrue(cmd.getOptionValue("b").Equals("foo"));
            Assert.AreEqual(1, cmd.getOptionValues("b").Length);

            Assert.IsTrue(cmd.hasOption("d"), "Option d is not set");
            Assert.IsTrue(cmd.getOptionValue("d").Equals("bar"));
            Assert.AreEqual(1, cmd.getOptionValues("d").Length);
        }

        [TestMethod]
        public void testMultipleArgValues()
        {
            Assert.IsTrue(cmd.hasOption("e"), "Option e is not set");
            CollectionAssert.AreEqual(new string[] { "one", "two" }, cmd.getOptionValues("e"));
        }

        [TestMethod]
        public void testTwoArgValues()
        {
            Assert.IsTrue(cmd.hasOption("g"), "Option g is not set");
            CollectionAssert.AreEqual(new string[] { "val1", "val2" }, cmd.getOptionValues("g"));
        }

        [TestMethod]
        public void testComplexValues()
        {
            Assert.IsTrue(cmd.hasOption("i"), "Option i is not set");
            Assert.IsTrue(cmd.hasOption("h"), "Option h is not set");
            CollectionAssert.AreEqual(new string[] { "val1", "val2" }, cmd.getOptionValues("h"));
        }

        [TestMethod]
        public void testExtraArgs()
        {
            CollectionAssert.AreEqual(new string[] { "arg1", "arg2", "arg3" }, cmd.getArgs(), "Extra args");
        }

        [TestMethod]
        public void testCharSeparator()
        {
            // tests the char methods of CommandLine that delegate to the string methods
            Assert.IsTrue(cmd.hasOption("j"), "Option j is not set");
            Assert.IsTrue(cmd.hasOption('j'), "Option j is not set");
            CollectionAssert.AreEqual(new string[] { "key", "value", "key", "value" }, cmd.getOptionValues("j"));
            CollectionAssert.AreEqual(new string[] { "key", "value", "key", "value" }, cmd.getOptionValues('j'));

            Assert.IsTrue(cmd.hasOption("k"), "Option k is not set");
            Assert.IsTrue(cmd.hasOption('k'), "Option k is not set");
            CollectionAssert.AreEqual(new string[] { "key1", "value1", "key2", "value2" }, cmd.getOptionValues("k"));
            CollectionAssert.AreEqual(new string[] { "key1", "value1", "key2", "value2" }, cmd.getOptionValues('k'));

            Assert.IsTrue(cmd.hasOption("m"), "Option m is not set");
            Assert.IsTrue(cmd.hasOption('m'), "Option m is not set");
            CollectionAssert.AreEqual(new string[] { "key", "value" }, cmd.getOptionValues("m"));
            CollectionAssert.AreEqual(new string[] { "key", "value" }, cmd.getOptionValues('m'));
        }

        /**
         * jkeyes - commented out this test as the new architecture
         * breaks this type of functionality.  I have left the test 
         * here in case I get a brainwave on how to resolve this.
         */
        /*
        public void testGetValue()
        {
            // the 'm' option
            Assert.IsTrue( _option.getValues().length == 2 );
            Assert.AreEqual( _option.getValue(), "key" );
            Assert.AreEqual( _option.getValue( 0 ), "key" );
            Assert.AreEqual( _option.getValue( 1 ), "value" );

            try {
                Assert.AreEqual( _option.getValue( 2 ), "key" );
                fail( "IndexOutOfBounds not caught" );
            }
            catch( IndexOutOfBoundsException exp ) {
            
            }

            try {
                Assert.AreEqual( _option.getValue( -1 ), "key" );
                fail( "IndexOutOfBounds not caught" );
            }
            catch( IndexOutOfBoundsException exp ) {

            }
        }
        */
    }
}
