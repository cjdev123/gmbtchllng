// To-do: 
// Do actual conversion of data
//
//Comments:
//Found an error during troubleshooting; the txt file and the GitHub readme.md have different values for the registers.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace Gambit
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
           // int counter = 0;


            //Read data 
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://tuftuf.gambitlabs.fi/feed.txt");
            StreamReader reader = new StreamReader(stream);


            System.IO.StreamReader file = new System.IO.StreamReader(stream);

            // Create list
            var RegList = new List<string>();


            // Read the file line by line.  
            while ((line = file.ReadLine()) != null)
            {

                string Id = line.Substring(0, line.IndexOf(":"));

                /*if (Id = "53") {
                   // do this
                }*/

                //Add line to list
                RegList.Add(line);
                

                // System.Console.WriteLine(line);
                //System.Console.WriteLine(Id);
                //Console.ReadKey();

            }

            //test - write item in list
            //string DateOfData = RegList[0];

            System.Console.WriteLine("##############################################");
            System.Console.WriteLine("ModBus Data registered " + RegList[0]);
            System.Console.WriteLine("##############################################");

            //Iterate through all items in list (could be based on number of items in list, instead of fixed 100)
            for (int i = 1; i < 100; i++) 
            
                {

                //CHECK CROSS REFERENCE FOR FORMAT!!!!!!!!!!!!!!!!!!!!!!!!!!!

                string a = RegList[i];

                //Console.WriteLine(a);
                string ItemIdInList = a.Substring(0, a.IndexOf(":"));
                //  Console.WriteLine(ItemIdInList);

                //Convert to int
                int newIdNumber;
                int.TryParse(ItemIdInList, out newIdNumber); 
                                                             

                //Console.ReadKey();

                if (newIdNumber == 51)
                {
                    //use only 1 register
                    string RegValue51 = a.Substring(a.IndexOf(":") + 1);
                    Console.WriteLine("Value of register " + newIdNumber + ": " + RegValue51);
                    //Convert this register 51
                    int value51 = Convert.ToInt32(RegValue51);
                    //Run conversion function
                   int returnedValue = convert(newIdNumber, value51, 0);
                    Console.WriteLine(returnedValue);
                    Console.WriteLine();
                }

                else if (newIdNumber > 55 && newIdNumber <= 72)
                {
                    //use only 1 register
                    string RegValue56to72 = a.Substring(a.IndexOf(":") + 1);
                    Console.WriteLine("Value of register " + newIdNumber + ": " + RegValue56to72);
                    //Convert this register
                    int value56to72 = Convert.ToInt32(RegValue56to72);
                    //Run conversion function
                    int returnedValue = convert(newIdNumber, value56to72, 0);
                    Console.WriteLine(returnedValue);
                    Console.WriteLine();
                }

                else if (newIdNumber > 91 && newIdNumber <= 96)
                {
                    //use only 1 register
                    string RegValue92to96 = a.Substring(a.IndexOf(":") + 1);
                    Console.WriteLine("Value of register " + newIdNumber + ": " + RegValue92to96);
                    //Convert this register
                    int value92to96 = Convert.ToInt32(RegValue92to96);
                    //Run conversion function
                    int returnedValue = convert(newIdNumber, value92to96, 0);
                    Console.WriteLine(returnedValue);
                    Console.WriteLine();
                }

                // In a real environment there would be more of these cases - like registers 158, 289 that use only 1 register, but there are only 100 lines in sample data
                else {
                    //use 2 registers
                      int newIdnumber2 = newIdNumber + 1;
                        
                        int newIdnumber2plus = newIdNumber + 1;
                        string firstReg = RegList[newIdNumber];
                        string secondReg = RegList[newIdnumber2];
                        

                        string RegValueOffirstReg = firstReg.Substring(firstReg.IndexOf(":") + 1);
                        string RegValueOfsecondReg = secondReg.Substring(secondReg.IndexOf(":") + 1);
                       // Console.WriteLine(RegList[99]);
                       // Console.WriteLine(RegList[100]);
                        Console.WriteLine("Values of registers " + newIdNumber + " and " + newIdnumber2 + ": " + RegValueOffirstReg + " and " + RegValueOfsecondReg);
                    //Console.WriteLine(secondReg);

                    //Convert the data in the two registers
                    int valueReg1 = Convert.ToInt32(RegValueOffirstReg);
                    int valueReg2 = Convert.ToInt32(RegValueOfsecondReg);
                    //Run conversion function
                    int returnedValue = convert(newIdNumber, valueReg1, valueReg2);
                    //Human readable form
                    Console.WriteLine("Converted data: " + returnedValue);
                    Console.WriteLine();
                    i++;
                    if (newIdnumber2 == 100)
                        {
                            break;
                        }
                    }
                }
            Console.ReadKey();
        }

        //This is the function to convert the data
        public static int convert(int Reg, int InputA, int InputB) {
            ////For the register/registers that have format int/long. Starting with the listed registers.
            if ((new[] { 9, 13, 17, 21, 25, 29, 59, 60, 61, 63, 72, 92, 93, 94, 96 }).Contains(Reg)) {

                //the two inputs converted to Hexadecimal and subtracted
                var hex1 = InputA.ToString("X");
                var hex2 = InputB.ToString("X");
                //string converted to int
                int num1 = Int32.Parse(hex1, System.Globalization.NumberStyles.HexNumber);
                int num2 = Int32.Parse(hex2, System.Globalization.NumberStyles.HexNumber);
                //Subtract one from the other
                int returnValue = num1 - num2;
                //return the value from the function to the function call
                return returnValue;

            }

            //For the register/registers that have "BCD" Binary coded decimal
            else if ((new[] { 0 }).Contains(Reg))
            {
                int returnValue = 1;
                return returnValue;
            }

            //The other register/registers that have format real4/float
            else
            {
                int CalculatedValue = InputA + InputB;
                
                int returnValue = Convert.ToInt32(CalculatedValue);
                //int returnValue = 999;
                return returnValue;
            }
                            
            }


        }

    }


/*
Format = REAL4 (float)
unless = 9, 13, 17, 21, 25, 29, 59, 60, 61, 63, 72, 92, 93, 94, 96, 
*/