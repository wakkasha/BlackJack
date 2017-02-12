using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlackJackGame.Models
{
    public enum GameResult { Win = 1, Lose =-1, Draw = 0, Pending = 2 };
    public class Card
    {
        public string ID  { get; set; }
        public string Suit { get; set; }
        public int Value { get; set; }
        public Card (string id, string suit, int value)
        {
            ID = id;
            Suit = suit;
            Value = value; 
        }
    }
    public class Deck : Stack<Card>
    {
        public Deck(IEnumerable<Card> collection) : base(collection) { }
        public Deck():base(52) { }
        public Card this[int index]
        {
            get
            {
                Card item;

                if(index >= 0 && index <= this.Count - 1)
                {
                    item = this.ToArray()[index];
                }
                else
                {
                    item = null;
                }
                return item;
            }
        }

        public double Value
        {
            get
            {
                return BlackJackRules.HandValue(this);
            }
        }
    }
    public class Member
    {
        public Deck Hand;
        public Member()
        {
            Hand = new Deck();
        }
    }
    public static class BlackJackRules
    {
        public static string[] ids = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "A", "J", "K", "Q" };

        public static string[] suits = { "C", "D", "H", "S" };
        public static Deck NewDeck
        {
            get
            {
                Deck d = new Deck();
                int value;

                foreach (string suit in suits)
                {
                    foreach (string id in ids)
                    {
                        value = Int32.TryParse(id, out value) ? value : id == "A" ? 1 : 10;
                        d.Push(new Card(id, suit, value));
                    }
                }
                return d;
            }
        }
        public static Deck ShuffledDeck
        {
            get
            {
                return new Deck(NewDeck.OrderBy(card => System.Guid.NewGuid()).ToArray());
            }
        }
        public static double HandValue(Deck deck)
        {
            int val1 = deck.Sum(c => c.Value);
            double aces = deck.Count(c => c.Suit == "A");
            double val2 = aces > 0 ? val1 + (10 * aces) : val1;
            return new double[] { val1, val2 }
            .Select(handVal => new
            {
                handVal,
                weight = Math.Abs(handVal - 21) + (handVal > 21 ? 100 : 0)
            })
            .OrderBy(n => n.weight)
            .First().handVal;
        }
        public static bool CanDealerHit(Deck deck, int standLimit)
        {
            return deck.Value < standLimit;
        }
        public static bool CanPlayerHit(Deck deck)
        {
            return deck.Value < 21;
        }

        public static GameResult GetResult(Member player, Member dealer)
        {
            GameResult res = GameResult.Win;
            double playerValue = HandValue(player.Hand);
            double dealerValue = HandValue(dealer.Hand);

            if (playerValue <= 21)
            {
                if (playerValue != dealerValue)
                {
                    double closestValue = new double[] { playerValue, dealerValue }
                    .Select(handVal => new
                    {
                        handVal,
                        weight = Math.Abs(handVal - 21) + (handVal > 21 ? 100 : 0
                    )
                    })
                    .OrderBy(n => n.weight)
                    .First().handVal;
                    res = playerValue == closestValue ? GameResult.Win : GameResult.Lose;
                }
                else
                {
                    res = GameResult.Draw;
                }
            }
            else
            {
                res = GameResult.Lose;
            }
            return res;
        }
    }

    public class BlackJack
    {
        public Member Dealer = new Member();
        public Member Player = new Member();
        public GameResult Result { get; set; }

        public Deck MainDeck ;
        public int StandLimit { get; set; }
        public BlackJack(int dealerStandLimit)
        {
            Result = GameResult.Pending;
            StandLimit = dealerStandLimit;
            MainDeck = BlackJackRules.ShuffledDeck;
            Dealer.Hand.Clear();
            Player.Hand.Clear();

            for(int i = 0; ++i < 3;)
            {
                Dealer.Hand.Push(MainDeck.Pop());
                Player.Hand.Push(MainDeck.Pop());
            }
        }
        public void Hit()
        {
            if(BlackJackRules.CanPlayerHit(Player.Hand) && Result == GameResult.Pending)
            {
                Player.Hand.Push(MainDeck.Pop());
            }
        }
        public void Stand()
        {
            if(Result == GameResult.Pending)
            {
                while (BlackJackRules.CanDealerHit(Dealer.Hand, StandLimit))
                {
                    Dealer.Hand.Push(MainDeck.Pop());
                }
                Result = BlackJackRules.GetResult(Player, Dealer);
            }
        }
    }

}
