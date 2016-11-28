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
    public class GnuParserTest : ParserTestCase
    {
        [TestInitialize]
        public void setUp()
        {
            base.setUp();
            parser = new GnuParser();
        }

        public override void testDoubleDash2()
        {
            // not supported by the GnuParser
        }

        public override void testLongWithoutEqualSingleDash()
        {
            // not supported by the GnuParser
        }

        public override void testAmbiguousLongWithoutEqualSingleDash()
        {
            // not supported by the GnuParser
        }

        public override void testNegativeOption()
        {
            // not supported by the GnuParser (CLI-184)
        }

        public override void testLongWithUnexpectedArgument1()
        {
            // not supported by the GnuParser
        }

        public override void testLongWithUnexpectedArgument2()
        {
            // not supported by the GnuParser
        }

        public override void testShortWithUnexpectedArgument()
        {
            // not supported by the GnuParser
        }

        public override void testUnambiguousPartialLongOption1()
        {
            // not supported by the GnuParser
        }

        public override void testUnambiguousPartialLongOption2()
        {
            // not supported by the GnuParser
        }

        public override void testUnambiguousPartialLongOption3()
        {
            // not supported by the GnuParser
        }

        public override void testUnambiguousPartialLongOption4()
        {
            // not supported by the GnuParser
        }

        public override void testAmbiguousPartialLongOption1()
        {
            // not supported by the GnuParser
        }

        public override void testAmbiguousPartialLongOption2()
        {
            // not supported by the GnuParser
        }

        public override void testAmbiguousPartialLongOption3()
        {
            // not supported by the GnuParser
        }

        public override void testAmbiguousPartialLongOption4()
        {
            // not supported by the GnuParser
        }

        public override void testPartialLongOptionSingleDash()
        {
            // not supported by the GnuParser
        }

        public override void testBursting()
        {
            // not supported by the GnuParser
        }

        public override void testUnrecognizedOptionWithBursting()
        {
            // not supported by the GnuParser
        }

        public override void testMissingArgWithBursting()
        {
            // not supported by the GnuParser
        }

        public override void testStopBursting()
        {
            // not supported by the GnuParser
        }

        public override void testStopBursting2()
        {
            // not supported by the GnuParser
        }
    }
}
