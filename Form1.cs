// Dmitrovskis Martynas IFIN-2/2
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

/*--------------------------------------------------------------------------------------------
    U1-2. Krepšinis.
Krepšinio mokykloje treniruotes lankančių sąrašas yra tekstiniame faile: būsimo krepšininko 
vardas ir pavardė, amžius ir ūgis. Pirmoje eilutėje yra krepšinio mokyklos pavadinimas. Turime 
dviejų mokyklų duomenis.

Skaičiavimai:
    Raskite, koks būsimų krepšininkų amžiaus vidurkis ir koks ūgio vidurkis kiekvienoje 
    mokykloje.
    Surašykite į atskirą rinkinį visus abiejų mokyklų sportininkus, kurių ūgis didesnis už 
    vidurkį.
    Surikiuokite rezultatų sąrašą amžiaus didėjimo ir vardus abecėlės tvarka.
    Pašalinkite iš rezultatų sąrašo krepšininkus, kurių amžius yra didesnis už nurodytą 
    klaviatūra.
    Papildykite surikiuotą rezultatų sąrašą naujais krepšininkais, kurių ūgis didesnis, už 
    sudaryto sąrašo krepšininkų ūgio vidurkį. Duomenys yra atskirame faile. Pirmoje eilutėje 
    – vadybininko vardas ir pavardė.

Reikalavimai programai:
    Grafinė vartotojo sąsaja(GVS);
    Duomenų klasė(savybės, konstruktorius su parametrais, metodai CompareTo ir ToString);
    Visų veiksmų rezultatus užrašyti rezultatų faile ir parodyti ekrane(sąsajoje);
    List rinkinys ir jo metodai, .csv failo panaudojimas, MS Excel diagrama.
--------------------------------------------------------------------------------------------*/

namespace U1_2_krepsinis_list
{
    public partial class Form1 : Form
    {
        string[] CFd;
        string CFd2;
        string CFr;
        const string CFt = "..\\..\\task.txt";
        const string CFtut = "..\\..\\tutorial.txt";

        private List<Player> PlayerList1;
        private List<Player> PlayerList2;
        private List<Player> PlayerList3;
        private List<Player> NewPlayList = new List<Player>();
        string age;

        string school1, school2, manager;

        public Form1()
        {
            InitializeComponent();

            readAgeToolStripMenuItem.Enabled = true;
            readDataToolStripMenuItem.Enabled = false;
            readAdditionalDataToolStripMenuItem.Enabled = false;
            printToolStripMenuItem.Enabled = false;
            averageToolStripMenuItem.Enabled = false;
            formatToolStripMenuItem.Enabled = false;
            sortToolStripMenuItem.Enabled = false;
            removeToolStripMenuItem.Enabled = false;
            extraFormattingToolStripMenuItem.Enabled = false;
        }

        //=========================================================================
        // GRAPHIC USER INTERFACE METHODS
        //=========================================================================

        /// <summary>
        /// Actions of the "ReadAge" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readAgeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ageLabel.Text = "Amžius";
            age = playerAge.Text;

            if (Convert.ToInt32(age) >= 0)
            {
                readAgeToolStripMenuItem.Enabled = false;
                readDataToolStripMenuItem.Enabled = true;

                notificationLabel.Text = "Amžius įvestas!";
            }
            else
            {
                playerAge.Clear();
                notificationLabel.Text = "Klaidingai įvestas amžius, bandykite iš naujo!";
                ageLabel.Text = "Amžius";
                age = playerAge.Text;
                readAgeToolStripMenuItem.Enabled = false;
                readAgeToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Actions of the "Read" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Pasirinkite duomenų failus";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                CFd = openFileDialog1.FileNames;
                PlayerList1 = ReadPlayerList(CFd[0], out school1);
                PlayerList2 = ReadPlayerList(CFd[1], out school2);
            }

            readDataToolStripMenuItem.Enabled = false;
            readAdditionalDataToolStripMenuItem.Enabled = true;
            printToolStripMenuItem.Enabled = true;
            averageToolStripMenuItem.Enabled = true;
            formatToolStripMenuItem.Enabled = true;
            sortToolStripMenuItem.Enabled = true;
            removeToolStripMenuItem.Enabled = true;

            notificationLabel.Text = "Pradiniai duomenys nuskaityti iš failų\n "
                + CFd[0] + "\n" + CFd[1];
        }

        /// <summary>
        /// Actions of the "ReadAdditional" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void readAdditionalDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.openFileDialog2.Multiselect = false;
            openFileDialog2.Title = "Pasirinkite papildomą duomenų failą";
            DialogResult result = openFileDialog2.ShowDialog();

            if (result == DialogResult.OK)
            {
                CFd2 = openFileDialog2.FileName;
                PlayerList3 = ReadPlayerList(CFd2, out manager);
            }

            readAdditionalDataToolStripMenuItem.Enabled = false;
            extraFormattingToolStripMenuItem.Enabled = true;

            notificationLabel.Text = "Įvesti papildomi duomenys iš failo\n!" + CFd2;
        }

        /// <summary>
        /// Actions of the "Print" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.Title = "Pasirinkite rezultatų failą";
            DialogResult result = saveFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                CFr = saveFileDialog1.FileName;

                if (File.Exists(CFr))
                    File.Delete(CFr);

                PrintPlayerList(CFr, PlayerList1, school1);
                PrintPlayerList(CFr, PlayerList2, school2);
                PrintPlayerList(CFr, PlayerList3, manager);
            }

            string resultFile = File.ReadAllText(CFr);
            data.Text = resultFile;
            printToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Pradiniai duomenys faile\n" + CFr;
        }

        /// <summary>
        /// Actions of the "Average" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintAverage(CFr, PlayerList1, school1 + " žaidėjų vidutinis " +
                    "amžius ir ūgis:");

            PrintAverage(CFr, PlayerList2, school2 + " žaidėjų vidutinis " +
                "amžius ir ūgis:");

            string resultFile = File.ReadAllText(CFr);
            data.Text = resultFile;
            averageToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Vidurkiai apskaičiuoti!";
        }

        /// <summary>
        /// Actions of the "Format" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormatNew(PlayerList1, NewPlayList);
            FormatNew(PlayerList2, NewPlayList);

            PrintPlayerList(CFr, NewPlayList, "Naujai suformuotas " +
                        "žaidėjų sąrašas");

            string resultFile = File.ReadAllText(CFr);
            data.Text = resultFile;
            formatToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Naujas sąrašas suformuotas!";
        }

        /// <summary>
        /// Actions of the "Sort" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewPlayList.Sort();
            PrintPlayerList(CFr, NewPlayList, "Surikiuotas žaidėjų sąrašas");

            string resultFile = File.ReadAllText(CFr);
            data.Text = resultFile;
            sortToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Žaidėjai surikiuoti!";
        }

        /// <summary>
        /// Actions of the "Remove" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove(NewPlayList, age);

            if (NewPlayList.Count > 0)
            {
                PrintPlayerList(CFr, NewPlayList, "Sąrašas po išmetimo");
            }
            else
            {
                using (var fr = new StreamWriter(File.Open(CFr, FileMode.Append)))
                {
                    fr.WriteLine("Visi žaidėjai išmesti!!!");
                }
            }

            string resultFile = File.ReadAllText(CFr);
            data.Text = resultFile;
            removeToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Netinkami žaidėjai pašalinti iš sąrašo!";
        }

        /// <summary>
        /// Actions of the "ExtraFormatting" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void extraFormattingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Formatting(NewPlayList, PlayerList3);
            PrintPlayerList(CFr, NewPlayList, "Sąrašas po papildomo formatavimo");

            string resultFile = File.ReadAllText(CFr);
            data.Text = resultFile;
            extraFormattingToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Galutinis sąrašas suformuotas!";
        }

        /// <summary>
        /// Actions of the "Task" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void taskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.LoadFile(CFt, RichTextBoxStreamType.PlainText);

            notificationLabel.Text = "Galite susipažinti su užduotimi!";
        }

        /// <summary>
        /// Actions of the "Tutorial" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tutorialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            data.LoadFile(CFtut, RichTextBoxStreamType.PlainText);

            notificationLabel.Text = "Darbo su programa nurodymai!";
        }

        /// <summary>
        /// Actions of the "CreateCsv" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateCsv(NewPlayList, CFr);
            createCsvToolStripMenuItem.Enabled = false;

            notificationLabel.Text = "Sukurtas CSV failas!";
        }

        /// <summary>
        /// Actions of the "End" menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }


        //===================================================================
        // USER METHODS
        //===================================================================

        /// <summary>
        /// Reads players data from a file to a dynamic array
        /// </summary>
        /// <param name="fn">file name</param>
        /// <param name="schoolName">school name</param>
        /// <returns>the reference dynamic array of players data</returns>
        static List<Player> ReadPlayerList(string fn, out string schoolName)
        {
            List<Player> PlayList = new List<Player>();
            using (StreamReader reader = new StreamReader(fn,
                Encoding.GetEncoding(1257)))
            {
                string line;
                schoolName = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(';');
                    string nameSurname = parts[0];
                    int age = int.Parse(parts[1]);
                    double height = double.Parse(parts[2]);
                    Player player = new Player(nameSurname, age, height);
                    PlayList.Add(player);
                }
            }
            return PlayList;
        }

        /// <summary>
        /// Prints player data on file
        /// </summary>
        /// <param name="fn">file name</param>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <param name="comment">a comment</param>
        static void PrintPlayerList(string fn, List<Player> PlayList, string comment)
        {
            const string header =
                        "--------------------------------------\r\n"
                      + " Nr. Pavardė ir vardas   Amžius  Ūgis \r\n"
                      + "--------------------------------------";
            using (StreamWriter fr = new StreamWriter(File.Open(fn, FileMode.Append)))
            {
                if (PlayList.Count > 0)
                {
                    fr.WriteLine("\n " + comment);
                    fr.WriteLine(header);
                    for (int i = 0; i < PlayList.Count; i++)
                    {
                        Player player = PlayList[i];
                        fr.WriteLine("{0, 3} {1}", i + 1, player);
                    }
                    fr.WriteLine("--------------------------------------");
                }
                else
                    fr.WriteLine("Tuščias duomenų failas!");
            }
        }

        /// <summary>
        /// Finds the average age of all team members
        /// </summary>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <returns>average age</returns>
        static double AverageAge(List<Player> PlayList)
        {
            int sumAge = PlayList.Sum(item => item.Age);
            double average;

            if (PlayList.Count > 0)
                average = (double)sumAge / PlayList.Count;
            else
                average = 0.0;

            return sumAge / PlayList.Count;
        }

        /// <summary>
        /// Finds the average height of all team members
        /// </summary>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <returns>average height</returns>
        static double AverageHeight(List<Player> PlayList)
        {
            double sumHeight = PlayList.Sum(item => item.Height);
            double average;

            if (PlayList.Count > 0)
                average = (double)sumHeight / PlayList.Count;
            else
                average = 0.0;

            return sumHeight / PlayList.Count;
        }

        /// <summary>
        /// Prints the average age and height of a team
        /// </summary>
        /// <param name="fn">file name</param>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <param name="comment">a comment</param>
        static void PrintAverage(string fn,
            List<Player> PlayList, string comment)
        {
            using (StreamWriter fr = new StreamWriter(File.Open(fn, FileMode.Append)))
            {
                if (PlayList.Count > 0 && AverageAge(PlayList) > 0
                    && AverageHeight(PlayList) > 0)
                {
                    fr.WriteLine(comment);
                    fr.WriteLine("{0, 2:f0} metai, {1, 3:f2} metrai",
                        AverageAge(PlayList), AverageHeight(PlayList));
                }
                else
                    fr.WriteLine("Klaida duomenyse!");
            }
        }

        /// <summary>
        /// Formats a new list
        /// </summary>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <param name="NewPlayList">a new dynamic array of players data</param>
        static void FormatNew(List<Player> PlayList,
                              List<Player> NewPlayList)
        {
            int index = NewPlayList.Count;
            for (int i = 0; i < PlayList.Count; i++)
            {
                if (PlayList[i].Height > AverageHeight(PlayList))
                {
                    NewPlayList.Insert(index, PlayList[i]);
                }
            }
        }

        /// <summary>
        /// Removes players from a list
        /// </summary>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <param name="age">given age</param>
        static void Remove(List<Player> PlayList, string age)
        {
            int playerAge = Convert.ToInt32(age);
            PlayList.RemoveAll(item => item.Age > playerAge);
        }

        /// <summary>
        /// Another formatting method
        /// </summary>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <param name="NewPlayList">a new dynamic array of players data</param>
        static void Formatting(List<Player> FinalPlayList, List<Player> ThirdPlayList)
        {
            double averageHeight = AverageHeight(FinalPlayList);
            bool check;

            for (int i = 0; i < ThirdPlayList.Count; i++) 
            {
                check = false;
                if (ThirdPlayList[i].Height > averageHeight)
                {
                    for (int j = 0; j < FinalPlayList.Count; j++) 
                    {
                        if ((FinalPlayList[j].CompareTo(ThirdPlayList[i])) == 1)
                        {
                            FinalPlayList.Insert(j, ThirdPlayList[i]);
                            check = true;
                            break;
                        }
                    }
                    if (check == false) 
                    {
                        FinalPlayList.Insert(FinalPlayList.Count, ThirdPlayList[i]);
                    }
                }
            }
        }

        /// <summary>
        /// A method for creating CSV files
        /// </summary>
        /// <param name="PlayList">dynamic array of players data</param>
        /// <param name="fn">file name</param>
        static void CreateCsv(List<Player> PlayList, string fn) 
        {
            string csv = "C:\\Users\\Core_Nvidia\\Desktop" +
                "\\Programming2\\U1_2_krepsinis_list\\xyz.csv";
            String separator = ",";
            StringBuilder output = new StringBuilder();
            String[] headings = { "First and last name", "Age", "Height" };
            output.AppendLine(string.Join(separator, headings));

            foreach (Player player in PlayList)
            {
                String[] newLine = { player.NameSurname,
                    player.Age.ToString(), player.Height.ToString() };
                output.AppendLine(string.Join(separator, newLine));
            }

            try
            {
                File.AppendAllText(csv, output.ToString());
            }
            catch (Exception ex)
            {
                using (StreamWriter fr = new StreamWriter(File.Open(fn, FileMode.Append)))
                {
                    fr.WriteLine("Neišėjo sukurti CSV failo!");
                }
                return;
            }
        }

        // End USER METHODS
    }
}