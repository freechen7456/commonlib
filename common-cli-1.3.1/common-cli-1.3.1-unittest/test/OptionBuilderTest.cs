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
    public class OptionBuilderTest
    {
        [TestMethod]
        public void testCompleteOption()
        {
            Option simple = OptionBuilder.withLongOpt("simple option")
                                         .hasArg()
                                         .isRequired()
                                         .hasArgs()
                                         .withType(typeof(float))
                                         .withDescription("this is a simple option")
                                         .create('s');

            Assert.AreEqual("s", simple.getOpt());
            Assert.AreEqual("simple option", simple.getLongOpt());
            Assert.AreEqual("this is a simple option", simple.getDescription());
            Assert.AreEqual(simple.getType(), typeof(float));
            Assert.IsTrue(simple.hasArg());
            Assert.IsTrue(simple.isRequired());
            Assert.IsTrue(simple.hasArgs());
        }

        [TestMethod]
        public void testTwoCompleteOptions()
        {
            Option simple = OptionBuilder.withLongOpt("simple option")
                                         .hasArg()
                                         .isRequired()
                                         .hasArgs()
                                         .withType(typeof(float))
                                         .withDescription("this is a simple option")
                                         .create('s');

            Assert.AreEqual("s", simple.getOpt());
            Assert.AreEqual("simple option", simple.getLongOpt());
            Assert.AreEqual("this is a simple option", simple.getDescription());
            Assert.AreEqual(simple.getType(), typeof(float));
            Assert.IsTrue(simple.hasArg());
            Assert.IsTrue(simple.isRequired());
            Assert.IsTrue(simple.hasArgs());

            simple = OptionBuilder.withLongOpt("dimple option")
                                  .hasArg()
                                  .withDescription("this is a dimple option")
                                  .create('d');

            Assert.AreEqual("d", simple.getOpt());
            Assert.AreEqual("dimple option", simple.getLongOpt());
            Assert.AreEqual("this is a dimple option", simple.getDescription());
            Assert.AreEqual(typeof(string), simple.getType());
            Assert.IsTrue(simple.hasArg());
            Assert.IsTrue(!simple.isRequired());
            Assert.IsTrue(!simple.hasArgs());
        }

        [TestMethod]
        public void testBaseOptionCharOpt()
        {
            Option baseOption = OptionBuilder.withDescription("option description")
                                       .create('o');

            Assert.AreEqual("o", baseOption.getOpt());
            Assert.AreEqual("option description", baseOption.getDescription());
            Assert.IsTrue(!baseOption.hasArg());
        }

        [TestMethod]
        public void testBaseOptionStringOpt()
        {
            Option baseOption = OptionBuilder.withDescription("option description")
                                       .create("o");

            Assert.AreEqual("o", baseOption.getOpt());
            Assert.AreEqual("option description", baseOption.getDescription());
            Assert.IsTrue(!baseOption.hasArg());
        }

        [TestMethod]
        public void testSpecialOptChars()
        {
            // '?'
            Option opt1 = OptionBuilder.withDescription("help options").create('?');
            Assert.AreEqual("?", opt1.getOpt());

            // '@'
            Option opt2 = OptionBuilder.withDescription("read from stdin").create('@');
            Assert.AreEqual("@", opt2.getOpt());

            // ' '
            try
            {
                OptionBuilder.create(' ');
                Assert.Fail("IllegalArgumentException not caught");
            }
            catch (ArgumentException e)
            {
                // success
            }
        }

        [TestMethod]
        public void testOptionArgNumbers()
        {
            Option opt = OptionBuilder.withDescription("option description")
                                      .hasArgs(2)
                                      .create('o');
            Assert.AreEqual(2, opt.getArgs());
        }

        [TestMethod]
        public void testIllegalOptions()
        {
            // bad single character option
            try
            {
                OptionBuilder.withDescription("option description").create('"');
                Assert.Fail("IllegalArgumentException not caught");
            }
            catch (ArgumentException exp)
            {
                // success
            }

            // bad character in option string
            try
            {
                OptionBuilder.create("opt`");
                Assert.Fail("IllegalArgumentException not caught");
            }
            catch (ArgumentException exp)
            {
                // success
            }

            // valid option 
            try
            {
                OptionBuilder.create("opt");
                // success
            }
            catch (ArgumentException exp)
            {
                Assert.Fail("IllegalArgumentException caught");
            }
        }

        [TestMethod]
        public void testCreateIncompleteOption()
        {
            try
            {
                OptionBuilder.hasArg().create();
                Assert.Fail("Incomplete option should be rejected");
            }
            catch (ArgumentException e)
            {
                // expected

                // implicitly reset the builder
                OptionBuilder.create("opt");
            }
        }

        [TestMethod]
        public void testBuilderIsResettedAlways()
        {
            try
            {
                OptionBuilder.withDescription("JUnit").create('"');
                Assert.Fail("IllegalArgumentException expected");
            }
            catch (ArgumentException e)
            {
                // expected
            }
            Assert.IsNull(OptionBuilder.create('x').getDescription(), "we inherited a description");

            try
            {
                OptionBuilder.withDescription("JUnit").create();
                Assert.Fail("IllegalArgumentException expected");
            }
            catch (ArgumentException e)
            {
                // expected
            }
            Assert.IsNull(OptionBuilder.create('x').getDescription(), "we inherited a description");
        }
    }
}
