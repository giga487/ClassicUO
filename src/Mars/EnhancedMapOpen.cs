using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ClassicUO.Mars.EnhancedM
{
    class EnhancedMapOpen
    {

        private string pathUltima;
        private string strWorkPath;
        private string pathLauncher;
        private string pathEnhancedMapFolder;
        private string pathEnhancedMapExe;
        public  bool isMapInstalled { get; }
        private Process map;
        private status statusEnhanced = status.OFF;
        private int ID = -1;

        private enum status
        {
            ON,
            OFF
        };

        private void createMapProcess()
        {
            map = new Process();
            map.StartInfo.FileName = pathEnhancedMapExe;
            map.StartInfo.WorkingDirectory = pathEnhancedMapFolder;
            map.StartInfo.Arguments = "";
        }

        public EnhancedMapOpen()
        {
            pathUltima = System.Reflection.Assembly.GetExecutingAssembly().Location;
            strWorkPath = System.IO.Path.GetDirectoryName(pathUltima);

            pathLauncher = Path.GetFullPath(Path.Combine(strWorkPath, @"..\.."));

            try 
            {
                if (Directory.Exists((pathEnhancedMapFolder = Path.Combine(pathLauncher, @"UoMarsMap\"))))
                {
                    isMapInstalled = Directory.Exists(pathEnhancedMapFolder);
                    pathEnhancedMapExe = Path.GetFullPath(Path.Combine(pathEnhancedMapFolder, @".\EnhancedMap.exe"));
                }


            }
            catch (InvalidCastException e)
            {
                Console.WriteLine("Errore Map Enhanced");
            }
            //

        }

        public bool setStatus()  //open True, closed False
        {

            
            if (statusEnhanced == status.OFF || checkProcessMapClosed()) //se chiusa, oppure se map is closed
            {
                startEnhancedMap();
                return true;
            }
            else
            {
                killEnhancedMap();
                return false;
            }
        }


        public int getProcId(Process target)
        {
            try //Se ci fosse qualche problema
            {
                if (target.Id > 0)
                    return target.Id;

                return target.Id; //questo perchè ho bisogno di tornare qualcosa;
            }
            catch (InvalidOperationException ex)
            {
                return -1;
            }

        }

        public bool killEnhancedMap()
        {

            if(!checkProcessMapClosed())
            {
                map.CloseMainWindow();
                map.Kill();
            }

            statusEnhanced = status.OFF;

            return true;
        }

        //Con questo metodo controllo che il processo sia stato chiuso, altrimenti forzo
        public bool checkProcessMapClosed() 
        {

            Process[] allProcess = Process.GetProcesses();

            foreach(var pro in allProcess)
            {
                try
                {
                    if (pro.Id == map.Id)
                        return false;

                }
                catch
                {
                    return true;
                }
            }

            return true;
        }

        public bool startEnhancedMap()
        {


            if(isMapInstalled)
            {
                createMapProcess();

                map.Start();

                if(getProcId(map) > -1)
                {
                    ID = getProcId(map);
                    statusEnhanced = status.ON;
                }


                return true;
            }



            return false;
        }



    }
}
