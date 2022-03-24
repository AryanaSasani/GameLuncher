
using System.Diagnostics;

namespace GameControl
{
    public partial class Form1 : Form
    {

        //impiments

        List<LogData> logData = new List<LogData>();  //sqlite !
        List<GameData> gameDataList = new List<GameData>();// Games sqliteDB

        int LogCount = 0;
        int GameIndexToAdd = 0;
        string GameAddressToDelete ="None";
   

        TimeSpan timeSpan = new TimeSpan(0, 0, 0);
        TimeSpan CurrentTime = new TimeSpan(0, 0, 0);
        int Today = 0;
        string RechargTime = "2:00:00";

        int processID = 0;
        bool GamesLocked = false;

        Process process1 = new Process();

        List<PictureBox> pictureBoxList = new List<PictureBox>();
        MyFunctions myFunctions = new MyFunctions();
        //
        public Form1()
        {
            InitializeComponent();
            button_DeleteGame.Enabled = false;

            Today = DateTime.Today.Day;

            UpdateLogsList(Today);


            //Games _ Pucture box
           
            for (int i = 1; i <= 5; i++)
            {
                pictureBoxList.Add((PictureBox)Controls.Find("pictureBox" + i, true)[0]);
                pictureBoxList[i-1].Enabled = false;
            }


            LoadGameList();
            if (gameDataList.Count > 0)
                GameIndexToAdd = gameDataList.Count ;
            else
                GameIndexToAdd = 0;

            LoadMyGames();


            //Thread GameTimerThread = new Thread(myFunctions.GaneTimer()) ;
            //GameTimerThread.Start();


            timer1.Enabled = true;
        }

        private void button_Exit_Click(object sender, EventArgs e)
        {
            SaveLogsListWhileExit();
            int i= gameDataList.Count();

            if (processID != 0)
            {

                var myprocess = Process.GetProcessById(processID);
                myprocess.Kill();
            }
           
            Application.Exit();

        }







        // functions
        private void LoadMyGames()
        {
            gameDataList.Clear();
            LoadGameList();
            int j = 0;
            for (int i = 1; i <= 5; i++)
            {
                if (i > gameDataList.Count)
                {
                    for(int y = i; y <= 5; y++)
                    {
                        pictureBoxList[y-1].Tag ="";
                        pictureBoxList[y - 1].Image = null;
                    }
                    break;
                }
                if (gameDataList[i - 1].GameNumber != null)
                {
                    //add game in UI
                    Icon result = Icon.ExtractAssociatedIcon(gameDataList[i - 1].Path);
                    pictureBoxList[j].Enabled = true;
                    pictureBoxList[j].Image = result.ToBitmap();
                    pictureBoxList[j].Tag = gameDataList[i - 1].Path;

                    j++;

                }

            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        { 

            label2.Text = timeSpan.ToString();

            if (timeSpan.ToString() != "00:00:00")
            {
                timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1));





            }
            else
            {
                if (GamesLocked == false)
                {
                    //lock games:
                    for (int i = 0; i < 5; i++)
                    {
                        pictureBoxList[i].Enabled = false;
                    }
                }

                if (process1.StartInfo.FileName != "")
                {


                    if (myFunctions.isRunning(process1.Id))
                    {
                        process1.Kill();

                    }

                    else
                    {
                    }

                }
                else
                {


                }
            }
        }


        private void LoadLogsList()
        {
            logData = SqliteDataAccess.LoadDB();

        }
        private void LoadGameList()
        {
            gameDataList = SqliteDataAccess.LoadGameDB();

        }
        private void SaveLogsListWhileExit()
        {
            logData = SqliteDataAccess.SelectDayDB(Today);//last logData
            LogData NewLogData = new LogData();
            NewLogData = logData[0];

            NewLogData.LastLifeHour = timeSpan.Hours;
            NewLogData.LastLifeMinute = timeSpan.Minutes;
            NewLogData.LastLifeSeccond = timeSpan.Seconds;
            SqliteDataAccess.ClearDB();
            SqliteDataAccess.SaveData(NewLogData);


        }
        private void UpdateLogsList(int CurrentDay)// When you LogIn 
        {

            logData = SqliteDataAccess.SelectDayDB(CurrentDay);
            if (logData.Count != 0)
            {
                int oldCount = logData[0].LogCount;
                if (oldCount != 0)
                {
                    SqliteDataAccess.UpdateData(oldCount + 1, CurrentDay.ToString());


                    // Get Remaining Life
                    int Hour = logData[0].LastLifeHour;
                    int Minute = logData[0].LastLifeMinute;
                    int Second = logData[0].LastLifeSeccond;

                    TimeSpan NewTimeSpan = new TimeSpan(Hour, Minute, Second);
                    timeSpan = NewTimeSpan;

                }
                /*  else
                  {
                      SqliteDataAccess.UpdateData(1, CurrentDay.ToString());
                      RechargTime = "9:00:00";
                  }*/

            }
            else
            {
                LogData NewLogData = new LogData();
                NewLogData.Day = CurrentDay;
                NewLogData.LogCount = 1;
                NewLogData.LastLifeHour = 2;
                NewLogData.LastLifeMinute = 0;
                NewLogData.LastLifeSeccond = 0;
                SqliteDataAccess.ClearDB();
                SqliteDataAccess.SaveData(NewLogData);

                //set Remaining Life
                TimeSpan NewTimeSpan = new TimeSpan(2, 0, 0);
                timeSpan = NewTimeSpan;

            }

        }


      



        private void button_AddGame_Click(object sender, EventArgs e)
        {
            
            // Add Game 
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Find your game";
            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK) // check if selected a file
            {
                string FilenamePath = openFileDialog.FileName;
                //add game in UI
                Icon result = Icon.ExtractAssociatedIcon(FilenamePath);
                pictureBoxList[GameIndexToAdd].Enabled = true;
                pictureBoxList[GameIndexToAdd].Image = result.ToBitmap();
                pictureBoxList[GameIndexToAdd].Tag = FilenamePath;
               

                //add game in Data Base;
                GameData gameData = new GameData();
                gameData.GameNumber = GameIndexToAdd;
                gameData.Name = openFileDialog.SafeFileName;
                gameData.Path = FilenamePath;

                SqliteDataAccess.SaveGameData(gameData);
                
                
                GameIndexToAdd++;
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox Clicked_picBox = (PictureBox)sender;
            string GameAddress = Clicked_picBox.Tag.ToString();


            // process1 = Process.Start(GameAddress);

            process1.StartInfo.FileName = GameAddress;
            process1.StartInfo.UseShellExecute = true;
            process1.StartInfo.Verb = "runas";
            process1.Start();
            processID = process1.Id;
         
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            SqliteDataAccess.Delete_Selected_Game(GameAddressToDelete);
            GameAddressToDelete = "None";
            button_DeleteGame.Enabled = false;
            
       
          
            LoadGameList();
            if (gameDataList.Count > 0)
                GameIndexToAdd = gameDataList.Count;
           else
                GameIndexToAdd = 0;

            LoadMyGames();


            // adjust size of Icon:
            for (int i = 0; i < 5; i++)
            {
                pictureBoxList[i].SizeMode = PictureBoxSizeMode.CenterImage;
            }
        }

        private void PictureBoxSelect_ToDelete(object sender, EventArgs e)
        {
            PictureBox Clicked_picBox = (PictureBox)sender;
                        
            GameAddressToDelete = Clicked_picBox.Tag.ToString();//GAME Adress to delete!
            button_DeleteGame.Enabled = true ;

            // adjust size of Icon:
            for (int i=0; i < 5; i++)
            {
                pictureBoxList[i].SizeMode = PictureBoxSizeMode.CenterImage;
            }
            Clicked_picBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void Form_ClickEvent(object sender, EventArgs e)
        {
            // adjust size of Icon:
            for (int i = 0; i < 5; i++)
            {
                pictureBoxList[i].SizeMode = PictureBoxSizeMode.CenterImage;
            }
            // disable Delete Key:
            button_DeleteGame.Enabled = false;

        }
    }
}