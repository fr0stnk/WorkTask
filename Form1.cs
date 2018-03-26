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
        

        public Form1()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var ws1 = new WorkStand();

            var algorythmThread = new Thread(ws1.AlgorythmStart);
            algorythmThread.Start();

            // Массив VE(1-5) лейблов состояний для сокращения повторяющегося кода
            labelsOfBools = new Label[]
            {
                VE1CondLabel,
                VE2CondLabel,
                VE3CondLabel,
                VE4CondLabel,
                VE5CondLabel
            };

            UpdateForm();
            
        }

        
        public void UpdateForm()
        {
            // Присвоение лейблам состояний значений в зависимости от значений VE в классе WorkStand (открыт или закрыт)
            for (var i = 0; i < labelsOfBools.Length; i++)
            {
                labelsOfBools[i].Text = (bool) WorkStand.VeList[i] ? "Открыт" : "Закрыт";
                labelsOfBools[i].ForeColor = (labelsOfBools[i].Text == "Открыт") ? Color.DarkOliveGreen : Color.Red;
            }

            // Обновление состояния прибора np 
            NPCondLabel.Text = WorkStand.Np1 ? "Включен" : "Выключен";

            // Обновление лейблов остальных приборов со значениями
            PE1ValueLabel.Text = Convert.ToString(WorkStand.GetPLC("PE1"));
            PE2ValueLabel.Text = Convert.ToString(WorkStand.GetPLC("PE2"));
            BPValueLabel.Text = Convert.ToString(WorkStand.GetPLC("BP"));
            GTValueLabel.Text = Convert.ToString(WorkStand.GetPLC("GT"));
        }
    }
}
