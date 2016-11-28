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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.commons.cli;
namespace org.apache.commons.cli.bug
{
    [TestClass]
    public class BugCLI162Test
    {
        /** Constant for the line separator.*/
        private static readonly String CR = Environment.NewLine;

        private HelpFormatter formatter;
        private StringWriter sw;

        [TestInitialize]
        public void setUp()
        {
            formatter = new HelpFormatter();
            sw = new StringWriter();
        }

        [TestMethod]
        public void testInfiniteLoop()
        {
            Options options = new Options();
            options.addOption("h", "help", false, "This is a looooong description");
            // used to hang & crash
            formatter.printHelp(sw, 20, "app", null, options, HelpFormatter.DEFAULT_LEFT_PAD, HelpFormatter.DEFAULT_DESC_PAD, null);

            String expected = "usage: app" + CR +
                    " -h,--help   This is" + CR +
                    "             a" + CR +
                    "             looooon" + CR +
                    "             g" + CR +
                    "             descrip" + CR +
                    "             tion" + CR;
            Assert.AreEqual(expected, sw.ToString());
        }

        [TestMethod]
        public void testPrintHelpLongLines()
        {
            // Constants used for options
            String OPT = "-";

            String OPT_COLUMN_NAMES = "l";

            String OPT_CONNECTION = "c";

            String OPT_DESCRIPTION = "e";

            String OPT_DRIVER = "d";

            String OPT_DRIVER_INFO = "n";

            String OPT_FILE_BINDING = "b";

            String OPT_FILE_JDBC = "j";

            String OPT_FILE_SFMD = "f";

            String OPT_HELP = "h";

            String OPT_HELP_ = "help";

            String OPT_INTERACTIVE = "i";

            String OPT_JDBC_TO_SFMD = "2";

            String OPT_JDBC_TO_SFMD_L = "jdbc2sfmd";

            String OPT_METADATA = "m";

            String OPT_PARAM_MODES_INT = "o";

            String OPT_PARAM_MODES_NAME = "O";

            String OPT_PARAM_NAMES = "a";

            String OPT_PARAM_TYPES_INT = "y";

            String OPT_PARAM_TYPES_NAME = "Y";

            String OPT_PASSWORD = "p";

            String OPT_PASSWORD_L = "password";

            String OPT_SQL = "s";

            String OPT_SQL_L = "sql";

            String OPT_SQL_SPLIT_DEFAULT = "###";

            String OPT_SQL_SPLIT_L = "splitSql";

            String OPT_STACK_TRACE = "t";

            String OPT_TIMING = "g";

            String OPT_TRIM_L = "trim";

            String OPT_USER = "u";

            String OPT_WRITE_TO_FILE = "w";

            String _PMODE_IN = "IN";

            String _PMODE_INOUT = "INOUT";

            String _PMODE_OUT = "OUT";

            String _PMODE_UNK = "Unknown";

            String PMODES = _PMODE_IN + ", " + _PMODE_INOUT + ", " + _PMODE_OUT + ", " + _PMODE_UNK;

            // Options build
            Options commandLineOptions;
            commandLineOptions = new Options();
            commandLineOptions.addOption(OPT_HELP, OPT_HELP_, false, "Prints help and quits");
            commandLineOptions.addOption(OPT_DRIVER, "driver", true, "JDBC driver class name");
            commandLineOptions.addOption(OPT_DRIVER_INFO, "info", false, "Prints driver information and properties. If "
                + OPT
                + OPT_CONNECTION
                + " is not specified, all drivers on the classpath are displayed.");
            commandLineOptions.addOption(OPT_CONNECTION, "url", true, "Connection URL");
            commandLineOptions.addOption(OPT_USER, "user", true, "A database user name");
            commandLineOptions
                    .addOption(
                            OPT_PASSWORD,
                            OPT_PASSWORD_L,
                            true,
                            "The database password for the user specified with the "
                                + OPT
                                + OPT_USER
                                + " option. You can obfuscate the password with org.mortbay.jetty.security.Password, see http://docs.codehaus.org/display/JETTY/Securing+Passwords");
            commandLineOptions.addOption(OPT_SQL, OPT_SQL_L, true, "Runs SQL or {call stored_procedure(?, ?)} or {?=call function(?, ?)}");
            commandLineOptions.addOption(OPT_FILE_SFMD, "sfmd", true, "Writes a SFMD file for the given SQL");
            commandLineOptions.addOption(OPT_FILE_BINDING, "jdbc", true, "Writes a JDBC binding node file for the given SQL");
            commandLineOptions.addOption(OPT_FILE_JDBC, "node", true, "Writes a JDBC node file for the given SQL (internal debugging)");
            commandLineOptions.addOption(OPT_WRITE_TO_FILE, "outfile", true, "Writes the SQL output to the given file");
            commandLineOptions.addOption(OPT_DESCRIPTION, "description", true,
                    "SFMD description. A default description is used if omited. Example: " + OPT + OPT_DESCRIPTION + " \"Runs such and such\"");
            commandLineOptions.addOption(OPT_INTERACTIVE, "interactive", false,
                    "Runs in interactive mode, reading and writing from the console, 'go' or '/' sends a statement");
            commandLineOptions.addOption(OPT_TIMING, "printTiming", false, "Prints timing information");
            commandLineOptions.addOption(OPT_METADATA, "printMetaData", false, "Prints metadata information");
            commandLineOptions.addOption(OPT_STACK_TRACE, "printStack", false, "Prints stack traces on errors");
            Option option = new Option(OPT_COLUMN_NAMES, "columnNames", true, "Column XML names; default names column labels. Example: "
                + OPT
                + OPT_COLUMN_NAMES
                + " \"cname1 cname2\"");
            commandLineOptions.addOption(option);
            option = new Option(OPT_PARAM_NAMES, "paramNames", true, "Parameter XML names; default names are param1, param2, etc. Example: "
                + OPT
                + OPT_PARAM_NAMES
                + " \"pname1 pname2\"");
            commandLineOptions.addOption(option);
            //
            OptionGroup pOutTypesOptionGroup = new OptionGroup();
            String pOutTypesOptionGroupDoc = OPT + OPT_PARAM_TYPES_INT + " and " + OPT + OPT_PARAM_TYPES_NAME + " are mutually exclusive.";
            String typesClassName = typeof(System.Data.DbType).FullName;
            option = new Option(OPT_PARAM_TYPES_INT, "paramTypes", true, "Parameter types from "
                + typesClassName
                + ". "
                + pOutTypesOptionGroupDoc
                + " Example: "
                + OPT
                + OPT_PARAM_TYPES_INT
                + " \"-10 12\"");
            commandLineOptions.addOption(option);
            option = new Option(OPT_PARAM_TYPES_NAME, "paramTypeNames", true, "Parameter "
                + typesClassName
                + " names. "
                + pOutTypesOptionGroupDoc
                + " Example: "
                + OPT
                + OPT_PARAM_TYPES_NAME
                + " \"CURSOR VARCHAR\"");
            commandLineOptions.addOption(option);
            commandLineOptions.addOptionGroup(pOutTypesOptionGroup);
            //
            OptionGroup modesOptionGroup = new OptionGroup();
            String modesOptionGroupDoc = OPT + OPT_PARAM_MODES_INT + " and " + OPT + OPT_PARAM_MODES_NAME + " are mutually exclusive.";
            option = new Option(OPT_PARAM_MODES_INT, "paramModes", true, "Parameters modes ("
                + (int)System.Data.ParameterDirection.Input
                + "=IN, "
                + (int)System.Data.ParameterDirection.InputOutput
                + "=INOUT, "
                + (int)System.Data.ParameterDirection.Output
                + "=OUT, "
                + (int)System.Data.ParameterDirection.ReturnValue
                + "=Unknown"
                + "). "
                + modesOptionGroupDoc
                + " Example for 2 parameters, OUT and IN: "
                + OPT
                + OPT_PARAM_MODES_INT
                + " \""
                + (int)System.Data.ParameterDirection.Output
                + " "
                + (int)System.Data.ParameterDirection.Input
                + "\"");
            modesOptionGroup.addOption(option);
            option = new Option(OPT_PARAM_MODES_NAME, "paramModeNames", true, "Parameters mode names ("
                + PMODES
                + "). "
                + modesOptionGroupDoc
                + " Example for 2 parameters, OUT and IN: "
                + OPT
                + OPT_PARAM_MODES_NAME
                + " \""
                + _PMODE_OUT
                + " "
                + _PMODE_IN
                + "\"");
            modesOptionGroup.addOption(option);
            commandLineOptions.addOptionGroup(modesOptionGroup);
            option = new Option(null, OPT_TRIM_L, true,
                    "Trims leading and trailing spaces from all column values. Column XML names can be optionally specified to set which columns to trim.");
            option.setOptionalArg(true);
            commandLineOptions.addOption(option);
            option = new Option(OPT_JDBC_TO_SFMD, OPT_JDBC_TO_SFMD_L, true,
                    "Converts the JDBC file in the first argument to an SMFD file specified in the second argument.");
            option.setArgs(2);
            commandLineOptions.addOption(option);

            formatter.printHelp(sw, HelpFormatter.DEFAULT_WIDTH, this.GetType().FullName, null, commandLineOptions, HelpFormatter.DEFAULT_LEFT_PAD, HelpFormatter.DEFAULT_DESC_PAD, null);
            String expected = "usage: org.apache.commons.cli.bug.BugCLI162Test" + CR +
                    " -2,--jdbc2sfmd <arg>        Converts the JDBC file in the first argument" + CR +
                    "                             to an SMFD file specified in the second" + CR +
                    "                             argument." + CR +
                    " -a,--paramNames <arg>       Parameter XML names; default names are" + CR +
                    "                             param1, param2, etc. Example: -a \"pname1" + CR +
                    "                             pname2\"" + CR +
                    " -b,--jdbc <arg>             Writes a JDBC binding node file for the given" + CR +
                    "                             SQL" + CR +
                    " -c,--url <arg>              Connection URL" + CR +
                    " -d,--driver <arg>           JDBC driver class name" + CR +
                    " -e,--description <arg>      SFMD description. A default description is" + CR +
                    "                             used if omited. Example: -e \"Runs such and" + CR +
                    "                             such\"" + CR +
                    " -f,--sfmd <arg>             Writes a SFMD file for the given SQL" + CR +
                    " -g,--printTiming            Prints timing information" + CR +
                    " -h,--help                   Prints help and quits" + CR +
                    " -i,--interactive            Runs in interactive mode, reading and writing" + CR +
                    "                             from the console, 'go' or '/' sends a" + CR +
                    "                             statement" + CR +
                    " -j,--node <arg>             Writes a JDBC node file for the given SQL" + CR +
                    "                             (internal debugging)" + CR +
                    " -l,--columnNames <arg>      Column XML names; default names column" + CR +
                    "                             labels. Example: -l \"cname1 cname2\"" + CR +
                    " -m,--printMetaData          Prints metadata information" + CR +
                    " -n,--info                   Prints driver information and properties. If" + CR +
                    "                             -c is not specified, all drivers on the" + CR +
                    "                             classpath are displayed." + CR +
                    " -o,--paramModes <arg>       Parameters modes (1=IN, 3=INOUT, 2=OUT," + CR +
                    "                             6=Unknown). -o and -O are mutually exclusive." + CR +
                    "                             Example for 2 parameters, OUT and IN: -o \"2" + CR +
                    "                             1\"" + CR +
                    " -O,--paramModeNames <arg>   Parameters mode names (IN, INOUT, OUT," + CR +
                    "                             Unknown). -o and -O are mutually exclusive." + CR +
                    "                             Example for 2 parameters, OUT and IN: -O \"OUT" + CR +
                    "                             IN\"" + CR +
                    " -p,--password <arg>         The database password for the user specified" + CR +
                    "                             with the -u option. You can obfuscate the" + CR +
                    "                             password with" + CR +
                    "                             org.mortbay.jetty.security.Password, see" + CR +
                    "                             http://docs.codehaus.org/display/JETTY/Securi" + CR +
                    "                             ng+Passwords" + CR +
                    " -s,--sql <arg>              Runs SQL or {call stored_procedure(?, ?)} or" + CR +
                    "                             {?=call function(?, ?)}" + CR +
                    " -t,--printStack             Prints stack traces on errors" + CR +
                    "    --trim <arg>             Trims leading and trailing spaces from all" + CR +
                    "                             column values. Column XML names can be" + CR +
                    "                             optionally specified to set which columns to" + CR +
                    "                             trim." + CR +
                    " -u,--user <arg>             A database user name" + CR +
                    " -w,--outfile <arg>          Writes the SQL output to the given file" + CR +
                    " -y,--paramTypes <arg>       Parameter types from System.Data.DbType. -y" + CR +
                    "                             and -Y are mutually exclusive. Example: -y" + CR +
                    "                             \"-10 12\"" + CR +
                    " -Y,--paramTypeNames <arg>   Parameter System.Data.DbType names. -y and -Y" + CR +
                    "                             are mutually exclusive. Example: -Y \"CURSOR" + CR +
                    "                             VARCHAR\"" + CR;
            Assert.AreEqual(expected, sw.ToString());
        }

        [TestMethod]
        public void testLongLineChunking()
        {
            Options options = new Options();
            options.addOption("x", "extralongarg", false,
                                         "This description has ReallyLongValuesThatAreLongerThanTheWidthOfTheColumns " +
                                         "and also other ReallyLongValuesThatAreHugerAndBiggerThanTheWidthOfTheColumnsBob, " +
                                         "yes. ");

            formatter.printHelp(sw, 35, this.GetType().FullName, "Header", options, 0, 5, "Footer");
            String expected = "usage:" + CR +
                              "       org.apache.commons.cli.bug.B" + CR +
                              "       ugCLI162Test" + CR +
                              "Header" + CR +
                              "-x,--extralongarg     This" + CR +
                              "                      description" + CR +
                              "                      has" + CR +
                              "                      ReallyLongVal" + CR +
                              "                      uesThatAreLon" + CR +
                              "                      gerThanTheWid" + CR +
                              "                      thOfTheColumn" + CR +
                              "                      s and also" + CR +
                              "                      other" + CR +
                              "                      ReallyLongVal" + CR +
                              "                      uesThatAreHug" + CR +
                              "                      erAndBiggerTh" + CR +
                              "                      anTheWidthOfT" + CR +
                              "                      heColumnsBob," + CR +
                              "                      yes." + CR +
                              "Footer" + CR;
            Assert.AreEqual(expected, sw.ToString(), "Long arguments did not split as expected");
        }

        [TestMethod]
        public void testLongLineChunkingIndentIgnored()
        {
            Options options = new Options();
            options.addOption("x", "extralongarg", false, "This description is Long.");

            formatter.printHelp(sw, 22, this.GetType().FullName, "Header", options, 0, 5, "Footer");
            String expected = "usage:" + CR +
                              "       org.apache.comm" + CR +
                              "       ons.cli.bug.Bug" + CR +
                              "       CLI162Test" + CR +
                              "Header" + CR +
                              "-x,--extralongarg" + CR +
                              " This description is" + CR +
                              " Long." + CR +
                              "Footer" + CR;
            Assert.AreEqual(expected, sw.ToString(), "Long arguments did not split as expected");
        }

    }
}
