using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;


namespace WorkTask
{
    public partial class Form1 : Form
    {
        public static Label[] labelsOfBools;
        //public delegate void delUpdateUILabel(Label label, string text);
        

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Массив VE(1-5) лейблов состояний для сокращения повторяющегося кода
            labelsOfBools = new Label[]
            {
                VE1CondLabel,
                VE2CondLabel,
                VE3CondLabel,
                VE4CondLabel,
                VE5CondLabel
            };

            var ws1 = new WorkStand();

            //var algorythmThread = new Thread(ws1.AlgorythmStart);
            //var algorythStartThread = new Thread(ws1.AlgorythmStart);
            //algorythStartThread.Start();

            BackgroundWorker bw;
            bw = new BackgroundWorker();
            bw.DoWork += (obj, ea) => ws1.AlgorythmStart();
            bw.ProgressChanged += (obj, ea) => UpdateForm();
            bw.RunWorkerAsync();
        }

        
        public void UpdateForm()
        {
            //delUpdateUILabel DelUpdateUILabel = new delUpdateUILabel(UpdateUILabel);
            // Присвоение лейблам состояний значений в зависимости от значений VE в классе WorkStand (открыт или закрыт)
            for (var i = 0; i < labelsOfBools.Length; i++)
            {
                //labelsOfBools[i].BeginInvoke(DelUpdateUILabel, labelsOfBools,
                    //(bool) WorkStand.VeList[i] ? "Открыт" : "Закрыт");
                //VE1CondLabel.BeginInvoke(DelUpdateUILabel, VE1CondLabel, (bool)WorkStand.VeList[i] ? "Открыт" : "Закрыт");
                labelsOfBools[i].Text = (bool) WorkStand.VeList[i] ? "Открыт" : "Закрыт";
                labelsOfBools[i].ForeColor = (labelsOfBools[i].Text == "Открыт") ? Color.DarkOliveGreen : Color.Red;
            }
            
            //NPCondLabel.BeginInvoke(DelUpdateUILabel, NPCondLabel, WorkStand.Np1 ? "Включен" : "Выключен");

            // Обновление состояния прибора np 
            NPCondLabel.Text = WorkStand.Np1 ? "Включен" : "Выключен";

            // Обновление лейблов остальных приборов со значениями
            //PE1ValueLabel.BeginInvoke(DelUpdateUILabel, PE1ValueLabel, Convert.ToString(WorkStand.GetPLC("PE1")));
            PE1ValueLabel.Text = Convert.ToString(WorkStand.GetPLC("PE1"));
            //PE2ValueLabel.BeginInvoke(DelUpdateUILabel, PE2ValueLabel, Convert.ToString(WorkStand.GetPLC("PE2")));
            PE2ValueLabel.Text = Convert.ToString(WorkStand.GetPLC("PE2"));
            //BPValueLabel.BeginInvoke(DelUpdateUILabel, BPValueLabel, Convert.ToString(WorkStand.GetPLC("BP3")));
            BPValueLabel.Text = Convert.ToString(WorkStand.GetPLC("BP"));
            //GTValueLabel.BeginInvoke(DelUpdateUILabel, GTValueLabel, Convert.ToString(WorkStand.GetPLC("GT")));
            GTValueLabel.Text = Convert.ToString(WorkStand.GetPLC("GT"));
        }

        //public void UpdateUILabel(Label labelName, string labelString)
        //{
        //    labelName.Text = labelString;
        //}
    }
}
