using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace LuhnTester
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                MainMenu();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void MainMenu()
        {
            /*
            * Card Number Pass Luhn but not Check Digit: 1234567812345670
            * Neither Pass: 1234567812345678
            * The following number was taken from an infographic at https://www.101computing.net/is-my-credit-card-valid/luhn-algorithm/
            * Card Number Pass Both Validations: 4137894711755904
            */

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("This app serves to test credit card numbers against the Luhn Algorithm");
            Console.WriteLine("Please do not use this for nefarious purposes");
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("Please Enter a Card Number: ");
            var cardIn = Console.ReadLine();

            var step1 = LuhnStep_1(cardIn); //Step 1: Double every other digit
            var step2 = LuhnStep_2(step1); //Step 2: Math that shiz
            var step3 = LuhnStep_3(step2); //Step 3: Add all the numbers together
            //Check to see if the last digit is 0, if it's not it's a bad card! (Not sure if it's possible for a valid card to be over 100 for this number)
            if (step3 < 100 && step3.ToString().ToCharArray()[1] != '0')
            {
                Console.WriteLine("Sorry Bub, you have a bad card number!");
            }
            else if (step3 >= 100 && step3.ToString().ToCharArray()[2] != '0') //This elseif only exists because I don't know if the resulting sum of step 3 can exceed 100
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sorry Bub, you have a bad card number!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Card Number Passed!");
            }

            //Now we take the results of steps 1&2 and change it up for step 3 to perform the Check Digit Validation
            var step3cd = LuhnStepCD_3(step2); //Step 3: Add all the numbers together
            var step4cd = LuhnStepCD_4(step3cd, step1[15]); //Step 4: validate check digit

            if (step4cd == true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Card passed Check Digit Validation");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Card did NOT pass Check Digit Validation");
            }

            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Again? (Y/N): ");
            var keyPress = Console.ReadKey();

            switch (keyPress.Key)
            {
                case ConsoleKey.Y:
                    //I mean we really don't need to do anything, the loop will handle it... So I guess we'll clear the screen to prepare for the next loop
                    Console.Clear();
                    break;

                case ConsoleKey.N:
                    Environment.Exit(0);
                    break;
                
                default:
                    Console.WriteLine("Invalid Option.... Exiting");
                    Environment.Exit(0);
                    break;
            }
        }

        #region Luhn's Algorithm (No Check Digit)
        /// <summary>
        /// In this step we Double every other digit
        /// </summary>
        /// <param name="cardNumber">16 digit credit card number</param>
        /// <returns></returns>
        public static int[] LuhnStep_1(string cardNumber)
        {
            int[] splitCard = new int[16]; //We know Credit card numbers are 16 digits, soooo we can specify the array size here
            int counter = 0; //just a simple counter so we can determine odd/even numbered digits
            foreach (char digit in cardNumber.ToCharArray())
            {
                if (counter % 2 == 0)
                {
                    splitCard[counter] = (Convert.ToInt32(digit.ToString()) * 2); //Depending on the position, we multiply the number by 2
                }
                else
                {
                    splitCard[counter] = Convert.ToInt32(digit.ToString());//So it's funny how if you take char(4) and try to turn it into an int it's the actual character value huh? That's why we have to change it back to a string
                }

                counter++; //what you've never seen an iterator before?
            }

            return splitCard;
        }

        /// <summary>
        /// Take each number in the array and check to see if any of them are greater than 10
        /// If they are split the two numbers and add them together
        /// </summary>
        /// <param name="cardAsArray">Integer array of the card number</param>
        /// <returns></returns>
        public static int[] LuhnStep_2(int[] cardAsArray)
        {
            //Ok fun time for Maths! We need to look at each number in the array here and check to see if it is greater than (or equal to) 10
            for(int i= 0; i < cardAsArray.Length; i++ )
            {
                if (cardAsArray[i] >= 10) 
                {
                    //If the number was greater than 9, we split that into two new numbers and add them together. Isn't Math great?
                    var digitSplit = cardAsArray[i].ToString().ToCharArray();
                    cardAsArray[i] = Convert.ToInt32(digitSplit[0].ToString()) + Convert.ToInt32(digitSplit[1].ToString());
                }
            }

            return cardAsArray;
        }

        /// <summary>
        /// Final Step of Luhn's Algorithm
        /// Add all of the Math'd digits up to a grand total
        /// </summary>
        /// <param name="mathedCard">Integer array of the numbers being doubled and checked for > 10</param>
        /// <returns></returns>
        public static int LuhnStep_3(int[] mathedCard)
        {
            //Uhh I don't know what you thought you were going to find here, we are just iterating over the array and adding it all together to make one happy number
            int sum = 0;
            foreach (int i in mathedCard)
            {
                sum += i;
            }

            return sum;
        }

        #endregion

        #region Luhn's Algorithm (No Check Digit)
        /// <summary>
        /// Final Step of Luhn's Algorithm
        /// Add all of the Math'd digits up to a grand total
        /// </summary>
        /// <param name="mathedCard">Integer array of the numbers being doubled and checked for > 10</param>
        /// <returns></returns>
        public static int LuhnStepCD_3(int[] mathedCard)
        {
            //Uhh I don't know what you thought you were going to find here, we are just iterating over the array and adding it all together to make one happy number
            int sum = 0;
            for (int i = 0; i < mathedCard.Length -1; i++)
            {
                sum += mathedCard[i];
            }

            return sum;
        }

        /// <summary>
        /// Check Digit Validation
        /// </summary>
        /// <param name="sum">The Sum of all the digits minus the check digit in the previous step</param>
        /// <param name="checkDigit">The Last number of the card</param>
        /// <returns></returns>
        public static bool LuhnStepCD_4(int sum, int checkDigit)
        {
            bool validCard = false;
            int? magicNumber;
            int? mathNumber;
            var splitA = sum.ToString().ToCharArray();

            if (sum > 99)
            {
                magicNumber = Convert.ToInt32(splitA[2].ToString());
                mathNumber = 10 - magicNumber;

                if (mathNumber == 0 || mathNumber == checkDigit)
                {
                    validCard = true;
                }
            }
            else
            {
                magicNumber = Convert.ToInt32(splitA[1].ToString());
                mathNumber = 10 - magicNumber;

                if (mathNumber == 0 || mathNumber == checkDigit)
                {
                    validCard = true;
                }
            }

            return validCard;
        }

        #endregion

        #region Card Number Decoder

        public static Dictionary<string, string> DecodeCardNumber(string cardNumber)
        {
            var result = new Dictionary<string, string>();
            var cardNumberSplitInator = cardNumber.ToCharArray();
            
            //Industry
                switch (cardNumber[0]) {

                    case '0':
                        result.Add("Industry", $"{cardNumber[0]} - International Organization of Standards");
                        break;

                    case '1':
                    case '2':
                        result.Add("Industry", $"{cardNumber[0]} - Airlines");
                        break;

                    case '3':
                        result.Add("Industry", $"{cardNumber[0]} - Travel & Entertainment");
                        break;

                    case '4':
                    case '5':
                        result.Add("Industry", $"{cardNumber[0]} - Banking");
                        break;

                    case '6':
                        result.Add("Industry", $"{cardNumber[0]} - Banking/Merchandising");
                        break;

                    case '7':
                        result.Add("Industry", $"{cardNumber[0]} - Petroleum");
                        break;

                    case '8':
                        result.Add("Industry", $"{cardNumber[0]} - Healthcare & Communications");
                        break;

                    case '9':
                        result.Add("Industry", $"{cardNumber[0]} - National Government");
                        break;
                }

            //Visa or Mastercard 
            // There are others too but man this part gets crazy
            // TL;DR if your card number starts with 4 is a VISA card full stop
            // if it starts with 51-55 it's a Mastercard
            // There is also something called Visa Electron but those too start with a 4
            // This can be anything from the first 6 digits of the card to 
            // The second digit to the sixth digit



            //Account Number in the System
            // the next 9 digits on the card are your "Account ID"
            // They determine your actual account number link

            //Check Digit
            // Allegedly the check digit can be used as an extra verification step on the Luhn Algorithm
            // You would take the last digit out of the main calculation
            // Then proceed through the process and take the last digit of the sum
            // subtract it from 10 and this should allegedly be your check digit (Unless it's 0?)
            

            return result;
        }

        #endregion

    }
}
