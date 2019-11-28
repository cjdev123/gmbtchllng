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

            //Call function to create new database for storing the ModBus data
            DatabaseCreation();

            //Read data feed/txt file
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("http://tuftuf.gambitlabs.fi/feed.txt");
            StreamReader reader = new StreamReader(stream);
            System.IO.StreamReader file = new System.IO.StreamReader(stream);


            // Create list
            var RegList = new List<string>();


            // Read the file, iterate line by line
            while ((line = file.ReadLine()) != null)
            {
                
                //Get the id number
                string Id = line.Substring(0, line.IndexOf(":"));
               
                string Data = line.Substring(line.IndexOf(":")+1);

                //Add each line to list
                RegList.Add(line);

                //Call the function to add the data into database. 
                //Would be good to exclude first row that contains date&time - future development
                AddDatabaseEntry(Id, Data);
                
            }

            //Create stunning header...
            System.Console.WriteLine("##############################################");
            System.Console.WriteLine("ModBus Data registered " + RegList[0]);
            System.Console.WriteLine("##############################################");

            //Iterate through all the elements in the list. (This would be better if it was based on the actual number of elements in the list, instead of a fixed 100...)
            for (int i = 1; i < 100; i++)

            {
                //Get the content of the element in the list
                string a = RegList[i];

                //Get the ID-number, split at character ":"
                string ItemIdInList = a.Substring(0, a.IndexOf(":"));

                //Convert string to int
                int newIdNumber;
                int.TryParse(ItemIdInList, out newIdNumber);

                //Different ways to handle registers for "single-register" and "double-registers"
                if (newIdNumber == 51)
                {
                    //use only 1 register
                    string RegValue51 = a.Substring(a.IndexOf(":") + 1);
                    Console.WriteLine("Value of register " + newIdNumber + ": " + RegValue51);

                    //Convert string to int
                    int value51 = Convert.ToInt16(RegValue51);

                    //Run conversion function "convert()" with ID and value of the register
                    int returnedValue = convert(newIdNumber, value51, 0);
                    Console.WriteLine(returnedValue);
                    Console.WriteLine();
                }

                else if (newIdNumber > 55 && newIdNumber <= 72)
                {
                    //use only 1 register
                    string RegValue56to72 = a.Substring(a.IndexOf(":") + 1);
                    Console.WriteLine("Value of register " + newIdNumber + ": " + RegValue56to72);

                    //Convert string to int
                    int value56to72 = Convert.ToInt32(RegValue56to72);

                    //Run conversion function "convert()" with ID and value of the register
                    int returnedValue = convert(newIdNumber, value56to72, 0);
                    Console.WriteLine(returnedValue);
                    Console.WriteLine();
                }

                else if (newIdNumber > 91 && newIdNumber <= 96)
                {
                    //use only 1 register
                    string RegValue92to96 = a.Substring(a.IndexOf(":") + 1);
                    Console.WriteLine("Value of register " + newIdNumber + ": " + RegValue92to96);

                    //Convert string to int
                    int value92to96 = Convert.ToInt16(RegValue92to96);

                    //Run conversion function "convert()" with ID and value of the register
                    int returnedValue = convert(newIdNumber, value92to96, 0);
                    Console.WriteLine(returnedValue);
                    Console.WriteLine();
                }

                // In a real environment there would be more of these cases - like registers 158, 289 that use only 1 register, 
                // but there are only 100 lines in sample data, so we stop at 100 for this challenge...

                else {
                    //The rest of the cases use 2 registers...

                    //Get the ID of the second register (we already have the first register in variable "newIdNumber"
                    int newIdnumber2 = newIdNumber + 1;

                    //The contents of the list element
                    string firstReg = RegList[newIdNumber];
                    string secondReg = RegList[newIdnumber2];

                    //Get the actual data for those registers
                    string RegValueOffirstReg = firstReg.Substring(firstReg.IndexOf(":") + 1);
                    string RegValueOfsecondReg = secondReg.Substring(secondReg.IndexOf(":") + 1);

                    //Present the data in console
                    Console.WriteLine("Values of registers " + newIdNumber + " and " + newIdnumber2 + ": " + RegValueOffirstReg + " and " + RegValueOfsecondReg);


                    //Convert the data in the two registers from string to int
                    int valueReg1 = Convert.ToInt32(RegValueOffirstReg);
                    int valueReg2 = Convert.ToInt32(RegValueOfsecondReg);

                    //Run the conversion function on the data of the registers
                    int returnedValue = convert(newIdNumber, valueReg1, valueReg2);

                    //Human readable form...
                    Console.WriteLine("Converted data: " + returnedValue);
                    Console.WriteLine();

                    //add 1 to the counter "i", so we get the two registers that belong together correctly, instead of incorrect pairs of reigsters
                    i++;

                    //Once we have iterated through all pairs, break the iteration
                    if (newIdnumber2 == 100)
                    {
                        break;
                    }
                }
            }


        //Call function to write contents of the database table in the console
            Console.ReadKey();
            QryDatabase();

            //Wait for keystroke before closing
            Console.ReadKey();
        }


        //FUNCTIONS FOLLOW:

        //This is the function that will convert the data, the inputs are registry ID, and the data of 1 or 2 registers
        //Note! This function is incomplete.
        public static int convert(int Reg, int InputA, int InputB)
        {

            ////For the registers with format int/long. The listed registers:
            if ((new[] { 9, 13, 17, 21, 25, 29, 59, 60, 61, 63, 72, 92, 93, 94, 96 }).Contains(Reg))
            {

                //the two inputs converted to Hexadecimal string
                var hex1 = InputA.ToString("X");
                var hex2 = InputB.ToString("X");

                //hex string converted to int
                int num1 = Int16.Parse(hex1, System.Globalization.NumberStyles.HexNumber);
                int num2 = Int16.Parse(hex2, System.Globalization.NumberStyles.HexNumber);

                //Subtract one from the other
                int returnValue = num1 - num2;

                //return the value from the function to the function call
                return returnValue;

            }

            //For the register/registers with format "BCD" Binary coded decimal
            else if ((new[] { 49, 51, 53, 56 }).Contains(Reg))
            {
                string toBinary = Convert.ToString(InputA, 2).PadLeft(16, '0');
                int returnValue = Int16.Parse(toBinary, System.Globalization.NumberStyles.HexNumber);
                return returnValue;
            }

            //The other register/registers that have the format real4/float
            else
            {
                //the two inputs converted to Hexadecimal
                var hex1 = InputA.ToString("X");
                var hex2 = InputB.ToString("X");

                //string CalculatedValue = hex1 + hex2;
                int returnValue = 0;
                return returnValue;
            }

        }

        //Function to create SQLite database with table for the register data
        public static void DatabaseCreation()
        {
            // Create table if it doesn't exist
            string createTableQuery = @"CREATE TABLE IF NOT EXISTS [RegisterData] (
                          [ID] VARCHAR(2)  NULL,
                          [DATA] VARCHAR(5)  NULL
                          )";

            //database file - if the database will be used daily to store new data, then new DB should not be created, instead use the existing one.
            System.Data.SQLite.SQLiteConnection.CreateFile("database.db3");

            using (System.Data.SQLite.SQLiteConnection con = new System.Data.SQLite.SQLiteConnection("data source=database.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com = new System.Data.SQLite.SQLiteCommand(con))
                {
                    con.Open();                             // Initiate DB connection

                    com.CommandText = createTableQuery;     // DB creation query as command
                    com.ExecuteNonQuery();                  // Execute the command/DB creation

                    con.Close();        // Close the connection to the database
                }

                            }

        }

        //Function to add entries in database.
        //Future development: The table entries should include date/time of reading as field, to facilitate better functionality/ better data queries
        public static void AddDatabaseEntry(string RegID, string RegData)
        {
 
            using (System.Data.SQLite.SQLiteConnection con2 = new System.Data.SQLite.SQLiteConnection("data source=database.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com2 = new System.Data.SQLite.SQLiteCommand(con2))
                {
                    con2.Open();                             // Open the connection to the database
                    com2.CommandText = "INSERT INTO RegisterData (ID, DATA) Values ('"+RegID+"','"+RegData+"')";   // Add the data into DB (Date/Time to be added in later development)
                    com2.ExecuteNonQuery();      // Execute the query
                    con2.Close();        // Close the connection to the database
                }

            }
        }

        //Function to query SQLite database and display data in console
        public static void QryDatabase()
        {   
            Console.WriteLine("Data from database: RegisterData");
            using (System.Data.SQLite.SQLiteConnection con3 = new System.Data.SQLite.SQLiteConnection("data source=database.db3"))
            {
                using (System.Data.SQLite.SQLiteCommand com3 = new System.Data.SQLite.SQLiteCommand(con3))
                {
                    con3.Open();                             // Open the connection to the database
                    com3.CommandText = "Select * FROM RegisterData";      // Select all rows from our database table

                    using (System.Data.SQLite.SQLiteDataReader reader = com3.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            
                            Console.WriteLine(reader["ID"] + " : " + reader["DATA"]);     // Display the value of the key and value column for every row
                           // Console.ReadKey();
                        }
                    }
                    con3.Close();        // Close the connection to the database
                }

            }
        }

    }
}