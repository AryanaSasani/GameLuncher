using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;

namespace GameControl
{
    public class SqliteDataAccess
    {
        public static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
        //LOGs tabel
        public static List<LogData> LoadDB()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LogData>("select * from LOGs", new DynamicParameters());
                return output.ToList();
            }
        }
        public static List<LogData> SelectDayDB(int SelectedDay)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LogData>(" select * from LOGs WHERE Day = "+ SelectedDay.ToString(), new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveData(LogData logData)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into LOGs (Day,LogCount,LastLifeHour,LastLifeMinute,LastLifeSeccond) values (@Day,@LogCount,@LastLifeHour,@LastLifeMinute,@LastLifeSeccond)", logData);
            }
        }
       
        public static void UpdateData(int NewCount,string CurrentDay)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE LOGs SET LogCount = "+ NewCount.ToString()+ " WHERE Day= " + CurrentDay);
                
            }
        }


        public static void ClearDB()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("DELETE from LOGs");
            }
        }

        // Games Table
     
    

        public static void SaveGameData(GameData gameData)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Games (Name,Path) values (@Name,@Path)", gameData);
            }
        }
        public static List<GameData> LoadGameDB()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<GameData>("select * from Games", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void Delete_Selected_Game(string GameAddress)//index from zero to 4
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("DELETE FROM Games WHERE Path = " +"\""+ GameAddress+"\"");
            }
        }



    }
}
