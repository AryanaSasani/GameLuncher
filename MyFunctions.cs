using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GameControl
{
    public class MyFunctions
    {
       public  bool isRunning(int id)
        {
            try
            {
                Process.GetProcessById(id);
            }
            catch (InvalidOperationException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }

        public string RenewTimeLife(string currentTime, string LastRemainingLife, bool FirstRunOfDay,string TotalLifeSpan) 
        {
            string Life;
            if (FirstRunOfDay)
            {
                Life = TotalLifeSpan;

            }
            else
            {
                Life = LastRemainingLife;
            }
            
            return Life;
        }
    }
}
