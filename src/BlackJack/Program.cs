using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackJackGame.Models;

namespace BlackJackGame
{
    public class Program
    {
        public static void ShowStats(BlackJack bj)
        {
            //Displays the word dealer over the dealer's hand
            Console.WriteLine("Dealer");
              
            //loops through list to generate hand for dealer
            foreach (Card c in bj.Dealer.Hand)
            {
                //Format and shows the randomly generated card and suit
                Console.WriteLine(string.Format("{0}{1}", c.ID, c.Suit));
            }
            //Displays dealers hand value
            Console.WriteLine(bj.Dealer.Hand.Value);

            //Puts the player on a new line
            Console.WriteLine(Environment.NewLine);

            //Displays the word player over the player's hand
            Console.WriteLine("Player");

            //loops through list to generate hand for player
            foreach (Card c in bj.Player.Hand)
            {
                //Format and shows the randomly generated card and suit
                Console.WriteLine(string.Format("{0}{1}", c.ID, c.Suit));
            }
            //Displays the value of the player below to two cards drawn
            Console.WriteLine(bj.Player.Hand.Value);
            //puts the value on a new line
            Console.WriteLine(Environment.NewLine);
        }
        public static void Main(string[] args)
        {
            //Game starts 
            string input = "";
            //new 
            BlackJack bj = new BlackJack(17);
            ShowStats(bj);

            while(bj.Result == GameResult.Pending)
            {
              input = Console.ReadLine();
            if(input.ToLower() == "H")
                {
                    bj.Hit();
                    ShowStats(bj);
                }
                else
                {
                    bj.Stand();
                    ShowStats(bj);
                }
            }
            Console.WriteLine(bj.Result);
            Console.ReadLine();
        }
    }
}
