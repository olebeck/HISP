﻿using System;
using System.Collections.Generic;
using MySqlConnector;

namespace Horse_Isle_Server
{
    class Database
    {

        public static MySqlConnection db;

        public static void OpenDatabase()
        {
            db = new MySqlConnection("server=" + ConfigReader.DatabaseIP + ";user=" + ConfigReader.DatabaseUsername + ";password=" + ConfigReader.DatabasePassword+";database="+ConfigReader.DatabaseName);
            db.Open();
            string UserTable = "CREATE TABLE Users(Id INT, Username TEXT(16),Email TEXT(128),Country TEXT(128),SecurityQuestion Text(128),SecurityAnswerHash TEXT(128),Age INT,PassHash TEXT(128), Salt TEXT(128),Gender TEXT(16), Admin TEXT(3), Moderator TEXT(3))";
            string ExtTable = "CREATE TABLE UserExt(Id INT, X INT, Y INT, Money INT, BankBalance BIGINT,ProfilePage Text(1028), CharId INT, ChatViolations INT)";
            string MailTable = "CREATE TABLE Mailbox(IdTo INT, PlayerFrom TEXT(16),Subject TEXT(128), Message Text(1028), TimeSent INT)";
            string BuddyTable = "CREATE TABLE BuddyList(Id INT, IdFriend INT, Pending BOOL)";
            string WorldTable = "CREATE TABLE World(Time INT,Day INT, Year INT, Weather TEXT(64))";
            string DroppedTable = "CREATE TABLE DroppedItems(X INT, Y INT, ItemID INT)";

            try
            {

                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = UserTable;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e) {
                Logger.WarnPrint(e.Message);
            };

            try
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = ExtTable;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.WarnPrint(e.Message);
            };

            try
            {

                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = MailTable;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.WarnPrint(e.Message);
            };

            try
            {

                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = BuddyTable;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.WarnPrint(e.Message);
            };

            try
            {

                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = DroppedTable;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.WarnPrint(e.Message);
            };


            try
            {

                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = WorldTable;
                sqlCommand.ExecuteNonQuery();



                sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "INSERT INTO World VALUES(0,0,0,'SUNNY')";
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();

            }
            catch (Exception e)
            {
                Logger.WarnPrint(e.Message);
            };
        }


        public static void SetServerTime(int time, int day, int year)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "UPDATE World SET Time=@time,Day=@day,Year=@year";
            sqlCommand.Parameters.AddWithValue("@time", time);
            sqlCommand.Parameters.AddWithValue("@day", day);
            sqlCommand.Parameters.AddWithValue("@year", year);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }

        public static int GetServerTime()
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT Time FROM World";
            int serverTime = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return serverTime;
        }

        public static int GetServerDay()
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT Day FROM World";
            int serverTime = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return serverTime;
        }

        public static int GetServerYear()
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT Year FROM World";
            int creationTime = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return creationTime;
        }
        public static string GetWorldWeather()
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT Weather FROM World";
            string Weather = sqlCommand.ExecuteScalar().ToString();
            return Weather;
        }

        public static void SetWorldWeather(string Weather)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "UPDATE World SET Weather=@weather";
            sqlCommand.Parameters.AddWithValue("@weather", Weather);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }

        public static byte[] GetPasswordSalt(string username)
        {
            if (CheckUserExist(username))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Salt FROM Users WHERE Username=@name";
                sqlCommand.Parameters.AddWithValue("@name", username);
                sqlCommand.Prepare();
                string expectedHash = sqlCommand.ExecuteScalar().ToString();
                return Converters.StringToByteArray(expectedHash);
            }
            else
            {
                throw new KeyNotFoundException("Username " + username + " not found in database.");
            }
        }

        public static int CheckMailcount(int id)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT COUNT(1) FROM Mailbox WHERE IdTo=@id";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Prepare();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return count;
        }

        public static void AddMail(int toId, string fromName, string subject, string message)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            int epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            sqlCommand.CommandText = "INSERT INTO Mailbox VALUES(@toId,@from,@subject,@message,@time)";
            sqlCommand.Parameters.AddWithValue("@toId", toId);
            sqlCommand.Parameters.AddWithValue("@from", fromName);
            sqlCommand.Parameters.AddWithValue("@subject", subject);
            sqlCommand.Parameters.AddWithValue("@mesasge", message);
            sqlCommand.Parameters.AddWithValue("@time", epoch);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();


        }

        public static bool CheckUserExist(int id)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT COUNT(1) FROM Users WHERE Id=@id";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Prepare();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return count >= 1;
        }
        public static bool CheckUserExist(string username)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT COUNT(1) FROM Users WHERE Username=@name";
            sqlCommand.Parameters.AddWithValue("@name", username);
            sqlCommand.Prepare();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return count >= 1;
        }
        public static bool CheckUserExtExists(int id)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT COUNT(1) FROM UserExt WHERE Id=@id";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Prepare();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return count >= 1;
        }


        public static bool CheckUserIsModerator(string username)
        {
            if (CheckUserExist(username))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Moderator FROM Users WHERE Username=@name";
                sqlCommand.Parameters.AddWithValue("@name", username);
                sqlCommand.Prepare();
                string modStr = sqlCommand.ExecuteScalar().ToString();
                return modStr == "YES";
            }
            else
            {
                throw new KeyNotFoundException("Username " + username + " not found in database.");
            }
        }


        public static bool CheckUserIsAdmin(string username)
        {
            if (CheckUserExist(username))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Admin FROM Users WHERE Username=@name";
                sqlCommand.Parameters.AddWithValue("@name", username);
                sqlCommand.Prepare();
                string adminStr = sqlCommand.ExecuteScalar().ToString();
                return adminStr == "YES";
            }
            else
            {
                throw new KeyNotFoundException("Username " + username + " not found in database.");
            }
        }

        public static int GetBuddyCount(int id)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT COUNT(1) FROM BuddyList WHERE Id=@id OR IdFriend=@id AND Pending=false";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Prepare();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return count;
        }

        public static int[] GetBuddyList(int id)
        {
            if (GetBuddyCount(id) <= 0)
                return new int[0];      // user is forever alone.

            List<int> buddyList = new List<int>();

            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT Id,IdFriend FROM BuddyList WHERE Id=@id OR IdFriend=@id AND Pending=false";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Prepare();
            MySqlDataReader dataReader = sqlCommand.ExecuteReader();

            while(dataReader.Read())
            {
                int adder = dataReader.GetInt32(0);
                int friend = dataReader.GetInt32(1);
                if (adder != id)
                    buddyList.Add(adder);
                else if (friend != id)
                    buddyList.Add(adder);
            }

            return buddyList.ToArray();
        }

        public static bool IsPendingBuddyRequestExist(int id, int friendId)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "SELECT COUNT(1) FROM BuddyList WHERE (Id=@id AND IdFriend=@friendId) OR (Id=@friendid AND IdFriend=@Id) AND Pending=true";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Parameters.AddWithValue("@friendId", friendId);
            sqlCommand.Prepare();

            Int32 count = Convert.ToInt32(sqlCommand.ExecuteScalar());
            return count >= 1;
        }

        public static void RemoveBuddy(int id, int friendId)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "DELETE FROM BuddyList WHERE (Id=@id AND IdFriend=@friendId) OR (Id=@friendid AND IdFriend=@Id)";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Parameters.AddWithValue("@friendId", friendId);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }
        public static void AcceptBuddyRequest(int id, int friendId)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "UPDATE BuddyList SET Pending=false WHERE (Id=@id AND IdFriend=@friendId) OR (Id=@friendid AND IdFriend=@Id)";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Parameters.AddWithValue("@friendId", friendId);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }
        public static void AddPendingBuddyRequest(int id, int friendId)
        {
            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "INSERT INTO BuddyList VALUES(@id,@friendId,true)";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Parameters.AddWithValue("@friendId", friendId);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }
        public static void CreateUserExt(int id)
        {
            if (CheckUserExtExists(id)) // user allready exists!
                throw new Exception("Userid " + id + " Allready in userext.");

            MySqlCommand sqlCommand = db.CreateCommand();
            sqlCommand.CommandText = "INSERT INTO UserExt VALUES(@id,@x,@y,0,0,'',0,0)";
            sqlCommand.Parameters.AddWithValue("@id", id);
            sqlCommand.Parameters.AddWithValue("@x", Map.NewUserStartX);
            sqlCommand.Parameters.AddWithValue("@y", Map.NewUserStartY);
            sqlCommand.Prepare();
            sqlCommand.ExecuteNonQuery();
        }

        public static int GetUserid(string username)
        {
            if (CheckUserExist(username))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Id FROM Users WHERE Username=@name";
                sqlCommand.Parameters.AddWithValue("@name", username);
                sqlCommand.Prepare();
                int userId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return userId;
            }
            else
            {
                throw new KeyNotFoundException("Username " + username + " not found in database.");
            }
        }

        public static int GetPlayerCharId(int userId)
        {
            if (CheckUserExtExists(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT CharId FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                int CharId = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return CharId;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }

        public static void SetPlayerCharId(int charid, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET CharId=@charId WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@charId", charid);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }

        public static int GetPlayerX(int userId)
        {
            if (CheckUserExtExists(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT X FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                int X = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return X;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }

        public static void SetPlayerX(int x, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET X=@x WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@x", x);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }

        public static int GetPlayerY(int userId)
        {
            if (CheckUserExtExists(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Y FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                int Y = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return Y;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }

        public static int GetChatViolations(int userId)
        {
            if (CheckUserExtExists(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT ChatViolations FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                int violations = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return violations;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }


        public static void SetChatViolations(int violations, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET ChatViolations=@violations WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@violations", violations);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }
        public static void SetPlayerY(int y, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET Y=@y WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@y", y);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }

        public static void SetPlayerMoney(int money, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET Money=@money WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@money", money);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }
        public static int GetPlayerMoney(int userId)
        {
            if (CheckUserExtExists(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Money FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                int Money = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return Money;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }

        public static int GetPlayerBankMoney(int userId)
        {
            if (CheckUserExtExists(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT BankBalance FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                int BankMoney = Convert.ToInt32(sqlCommand.ExecuteScalar());
                return BankMoney;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }

        public static void SetPlayerBankMoney(int bankMoney, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET BankBalance=@bankMoney WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@bankMoney", bankMoney);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }

        public static void SetPlayerProfile(string profilePage, int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "UPDATE UserExt SET ProfilePage=@profilePage WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@profilePage", profilePage);
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                sqlCommand.ExecuteNonQuery();
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }

        public static string GetPlayerProfile(int id)
        {
            if (CheckUserExist(id))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT ProfilePage FROM UserExt WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", id);
                sqlCommand.Prepare();
                string profilePage = sqlCommand.ExecuteScalar().ToString();
                return profilePage;
            }
            else
            {
                throw new KeyNotFoundException("Id " + id + " not found in database.");
            }
        }


        public static string GetUsername(int userId)
        {
            if(CheckUserExist(userId))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT Username FROM Users WHERE Id=@id";
                sqlCommand.Parameters.AddWithValue("@id", userId);
                sqlCommand.Prepare();
                string username = sqlCommand.ExecuteScalar().ToString();
                return username;
            }
            else
            {
                throw new KeyNotFoundException("Id " + userId + " not found in database.");
            }
        }
        public static byte[] GetPasswordHash(string username)
        {
            if(CheckUserExist(username))
            {
                MySqlCommand sqlCommand = db.CreateCommand();
                sqlCommand.CommandText = "SELECT PassHash FROM Users WHERE Username=@name";
                sqlCommand.Parameters.AddWithValue("@name", username);
                sqlCommand.Prepare();
                string expectedHash = sqlCommand.ExecuteScalar().ToString();
                return Converters.StringToByteArray(expectedHash);
            }
            else
            {
                throw new KeyNotFoundException("Username " + username + " not found in database.");
            }
        }
    }

}