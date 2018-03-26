using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WorkTask
{
    public class WorkStand
    {
        // Значение PE1 (вакуумметр)
        // Значение PE2 (вакуумметр)
        private static float _pe2;
        // Значение BP3 (преобразователь давления)
        private static float _bp3;
        // Значение GT (течеискатель)
        private static float _gt;
        // Состояние NP1 (насос вакуумный пластинчато-роторный)
        // Состояние RD1 (регулятор давления)

        // Состояния GT (течеискателя)
        private static GtState currenntGtState;
        private enum GtState
        {
            On,
            Off,
            Diagn,
            Vent,
            Measure
        }

        //Переменная времени для отмерки времени выполнения
        DateTime _dt;
        
        // Состояния VE электромагнитные клапаны

        //Свойство доступа к PE1
        public static float Pe1 { get; private set; }

        // Свойство доступа к списку с VE(1-5) (Для работы метода UpdateForm в Form1.cs и инкапсуляции)
        public static List<bool?> VeList { get; private set; }

        //Свойство доступа NP1
        public static bool Np1 { get; private set; }

        //Свойство доступа к RD1
        public static bool Rd1 { get; private set; }

        
        // Конструктор класса
        public WorkStand()
        {
            // Тестовые данные
            Pe1 = 10;
            _pe2 = 10;
            _bp3 = 10;
            _gt = 10;
            
            // Добавление клапанов
            VeList = new List<bool?>();
            for (var i = 0; i < 12; i++)
            {
                VeList.Add(false);
            }

        }

        /// <summary>
        /// SendPLC («обозначение оборудования», ‘команда’) – функция возвращает «1»,
        /// Если команда выполнена без ошибок, «-1», если произошла ошибка передачи 
        /// Или ошибка выполнения команды.
        /// </summary>
        /// <param name="equipment"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static int SendPLC(string equipment, string command)
        {
            // Обработка VE
            if (equipment.Contains("VE"))
            {
                if (equipment.Length == 3)
                {
                    var indexator = int.Parse(equipment.Substring(equipment.Length - 1));
                    switch (command)
                    {
                        case "0":
                            VeList[indexator] = false;
                            return 1;
                        case "1":
                            VeList[indexator] = true;
                            return 1;
                        default:
                            return -1;

                    }
                }

                if (equipment.Length == 4)
                {
                    var indexator = int.Parse(equipment.Substring(equipment.Length - 2));
                    switch (command)
                    {
                        case "0":
                            VeList[indexator] = false;
                            return 1;
                        case "1":
                            VeList[indexator] = true;
                            return 1;
                        default:
                            return -1;

                    }
                }
            }

            //Обработка NP
            if (equipment.Contains("NP"))
            {
                switch (command)
                {
                    case "0":
                        Np1 = false;
                        return 1;
                    case "1":
                        Np1 = true;
                        return 1;
                    default:
                        return -1;
                }
            }

            //Обработка RD
            if (equipment.Contains("RD1"))
            {
                switch (command)
                {
                    case "0":
                        Rd1 = false;
                        return 1;
                    case "1":
                        Rd1 = true;
                        return 1;
                    default:
                        return -1;
                }
            }

            // Обработка остальных устройств
            if (equipment.Contains("GT"))
            {
                switch (command)
                {
                    case "on":
                        currenntGtState = GtState.On;
                        return 1;
                    case "off":
                        currenntGtState = GtState.Off;
                        return 1;
                    case "diagn":
                        currenntGtState = GtState.Diagn;
                        return 1;
                    case "vent":
                        currenntGtState = GtState.Vent;
                        return 1;
                    case "measure":
                        currenntGtState = GtState.Measure;
                        return 1;
                    default:
                        return -1;

                }
            }

            return -1;

        }

        /// <summary>
        /// GetPLC(“обозначение оборудования») – возвращает значения от 0 до 99.9.
        /// </summary>
        /// <param name="eqipment"></param>
        /// <returns></returns>
        public static float GetPLC(string eqipment)
        {
            switch (eqipment)
            {
                case "PE1":
                    return Pe1;
                case "PE2":
                    return _pe2;
                case "BP3":
                    return _bp3;
                case "GT":
                    return _gt;
                default:
                    return -1;
            }
        }

        // Алгоритм
        public async void AlgorythmStart()
        {

            // Инструкции
            // 2.1 шаг 
            SendPLC("VE1", "1");
            SendPLC("VE2", "1");
            //Thread.Sleep(0);

            // 2.2 шаг
            SendPLC("VE3", "0");
            SendPLC("VE5", "0");
            //Thread.Sleep(0);

            // 2.3 шаг
            SendPLC("NP1", "1");
            //Thread.Sleep(0);

            // 2.4 шаг
            SendPLC("NP1", "1");
            if (GetPLC("PE2") <= 20)
            {
                SendPLC("NP1", "0");
            }
            //Thread.Sleep(0);

            // 2.5 шаг
            SendPLC("VE2", "0");
            //Thread.Sleep(0);

            // 2.6 шаг
            SendPLC("NP1", "0");
            //Thread.Sleep(0);

            // 2.7 шаг
            SendPLC("VE3", "1");
            if (GetPLC("PE1") <= 99.0f)
            {
                SendPLC("VE3", "0");
            }
            //Thread.Sleep(0);

            // 2.8 шаг (автоматическая диагностика и переключение в режим «готов»)
            SendPLC("GT", "diagn");
            SendPLC("GT", "on");

            // 2.9 шаг
            SendPLC("GT", "measure");
            if (! (GetPLC("GT") < 50.7f))
            {
                MessageBox.Show("Показание потока гелия не в норме");
            }
            //Thread.Sleep(0);

            // 2.10 шаг
            SendPLC("VE6", "0");
            //Thread.Sleep(0);

            // 2.11 шаг 
            // Открыть электромагнитный клапан VE10 или VE11 
            // (включение в работу одного из двух баллонов газовых Б1 (рабочий) и Б2 резервный))
            // Контроль давления в баллонах происходит с помощью преобразователя давления ВР3 (VE10 для Б1, VE11 для Б2, контрольное значение  <15).

            if (GetPLC("BP3") < 15)
            {
                SendPLC("VE10", "1");
            }
            else
                SendPLC("VE11", "1");
            //Thread.Sleep(0);

            // 2.12 шаг
            SendPLC("VE8", "1");
            //Thread.Sleep(0);

            // 2.13 шаг
            SendPLC("RD1", "1");
            //Thread.Sleep(0);


            // 2.14 шаг 
            // Новый поток для контроля показаний потока гелия с ГТ в течение 30 секунд
            // выводить на экран значения, получаемые от ГТ
            _dt = DateTime.Now.AddSeconds(30);

            if (!(DateTime.Now == _dt))
            {
                GetPLC("GT");
            }
            //Thread.Sleep(0);


            // 2.15 шаг
            GetPLC("GT");
            //Вывод значения GT
            //Thread.Sleep(0);


            // 2.16 шаг
            SendPLC("VE8", "1");
            //Thread.Sleep(0);


            // 2.17 шаг
            SendPLC("VE6", "1");
            //Thread.Sleep(0);


            // 2.18 шаг
            SendPLC("RD1", "0");
            //Thread.Sleep(0);

            // 2.19 шаг
            // Переключить в состояние готов
            SendPLC("GT", "on");
            //Thread.Sleep(0);

            // 2.20 шаг
            SendPLC("VE1", "0");
            //Thread.Sleep(0);

            // 2.21 шаг
            SendPLC("VE5", "1");
            if (GetPLC("PE2") == 90)
            {
                SendPLC("VE5", "0");
            }
            //Thread.Sleep(0);
        }
    }
}
