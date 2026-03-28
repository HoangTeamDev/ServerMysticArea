using MySqlConnector;
using ServerMysticArea.CardData;
using ServerMysticArea.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerMysticArea.DB
{
    public class CardRepository
    {
      
        public PlayerDeckCard LoadDeckCards(long deckId)
        {
           
            var cards = new Dictionary<int, int>();

            using var conn = MySqlDb.GetConnection();
            conn.Open();

            string query = @"
        SELECT CardId, Quantity
        FROM DeckCard
        WHERE DeckId = @deckId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@deckId", deckId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int cardId = reader.GetInt32("CardId");
                int quantity = reader.GetInt32("Quantity");

                cards[cardId] = quantity;
            }
            PlayerDeckCard playerDeckCard = new PlayerDeckCard
            {
                DeckId = (int)deckId,
                Cards = cards
            };
            return playerDeckCard;
        }
        public List<PlayerDeck> LoadDecksByPlayerId(long playerId)
        {
            var decks = new List<PlayerDeck>();

            using var conn = MySqlDb.GetConnection();
            conn.Open();

            string query = @"
        SELECT Id, DeckName, IsActive
        FROM Deck
        WHERE PlayerId = @playerId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@playerId", playerId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                decks.Add(new PlayerDeck
                {
                    DeckId = reader.GetInt32("Id"),
                    DeckName = reader.GetString("DeckName"),
                    isActive = reader.GetBoolean("IsActive")
                });
            }

            return decks;
        }
        public Dictionary<int, int> LoadPlayerCardById(int playerId)
        {
            var playerCard = new Dictionary<int, int>();

            using var conn = MySqlDb.GetConnection();
            conn.Open();

            string query = @"
        SELECT CardId, Quantity
        FROM PlayerCard
        WHERE PlayerId = @playerId";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@playerId", playerId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int cardId = reader.GetInt32("CardId");
                int quantity = reader.GetInt32("Quantity");

                playerCard[cardId] = quantity;
            }

            return playerCard;
        }
        public Dictionary<int, Card> LoadAllCards()
        {
            var cards = new Dictionary<int, Card>();

            using var conn = MySqlDb.GetConnection();
            conn.Open();

            string query = @"SELECT ID, Name,CardType,Rarity,Cost
                         FROM cards";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var card = new Card
                {
                    _CardId = reader.GetInt32("ID"),
                    _Name = reader.GetString("Name"),
                    _CardType = reader.GetInt32("CardType"),
                    _Rarity = reader.GetString("Rarity"),
                    _Cost = reader.GetInt32("Cost"),
                };

                cards.Add(card._CardId, card);
            }

            reader.Close();
            string query1 = "SELECT CardId, KeywordId FROM CardKeywords";

            using var cmd1 = new MySqlCommand(query1, conn);
            using var reader1 = cmd1.ExecuteReader();

            while (reader1.Read())
            {
                int cardId = reader1.GetInt32("CardId");
                int keyword = reader1.GetInt32("KeywordId");

                if (cards.TryGetValue(cardId, out var card))
                {
                    card._KeyWord = keyword;
                }
            }

            reader1.Close();

            string query2 = "SELECT CardId, Level,ATK,HP FROM CardStats";

            using var cmd2 = new MySqlCommand(query2, conn);
            using var reader2 = cmd2.ExecuteReader();

            while (reader2.Read())
            {
                int cardId = reader2.GetInt32("CardId");
                int Level = reader2.GetInt32("Level");
                int atk = reader2.GetInt32("ATK");
                int HP = reader2.GetInt32("HP");

                if (cards.TryGetValue(cardId, out var card))
                {
                    card._Level = Level;
                    card._Hp = HP;
                    card._Attack = atk;
                }
            }
            reader2.Close();

            string query3 = "SELECT CardId, Race,Element FROM CardAttributes";

            using var cmd3 = new MySqlCommand(query3, conn);
            using var reader3 = cmd3.ExecuteReader();

            while (reader3.Read())
            {
                int cardId = reader3.GetInt32("CardId");
                int race = reader3.GetInt32("Race");
                int element = reader3.GetInt32("Element");


                if (cards.TryGetValue(cardId, out var card))
                {
                    card._Race = race;
                    card._Element = element;

                }
            }
            reader3.Close();
            string query4 = "SELECT CardId, Id,SkillName, Description, TriggerType, Onceturn, ActiveZone, TriggerMode FROM CardEffects";

            using var cmd4 = new MySqlCommand(query4, conn);
            using var reader4 = cmd4.ExecuteReader();

            while (reader4.Read())
            {
                CardEffects effects = new CardEffects();
                int cardId = reader4.GetInt32("CardId");
                effects._id = reader4.GetInt32("Id");
                effects._Skillname = reader4.GetString("SkillName");
                effects._Des = reader4.GetString("Description");
                effects._TriggerType = reader4.GetString("TriggerType");
                effects._OnePerTurn = reader4.GetBoolean("Onceturn");
                effects._ActiveZone = reader4.GetString("ActiveZone");
                effects._triggerMode = reader4.GetString("TriggerMode");


                if (cards.TryGetValue(cardId, out var card))
                {
                    card.CardEffects.Add(effects);

                }
            }
            reader4.Close();
            return cards;
        }
    }
}
