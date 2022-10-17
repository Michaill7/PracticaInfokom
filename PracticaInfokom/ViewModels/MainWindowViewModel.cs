using PersonnelAccountingSystem.ViewModels.Base;
using PracticaInfokom.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PracticaInfokom.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private DataTable crats;
        private DataTable slyabs;

        public DataTable Slyabs
        {
            get => slyabs;
            set
            {
                slyabs = value;
                OnPropertyChange();
            }
        }

        public DataTable Crats
        {
            get => crats;
            set
            {
                crats = value;
                OnPropertyChange();
            }
        }

        private string datebegin = DateTime.Now.ToString();

        public string DateBegin
        {
            get => datebegin;
            set
            {
                datebegin = value;
            }
        }

        private string dateend = DateTime.Now.ToString();

        public string DateEnd
        {
            get => dateend;
            set
            {
                dateend = value;
            }
        }

        public ICommand UpdateButton { get; }

        private bool UpdateButtonCanExecute(object o) => true;
        private void UpdateBUttonOnExecuted(object o)
        {
            //Получение даты и времени с интерфейса и конвертирование в DateTime
            DateTime upd_begin;
            DateTime.TryParse(DateBegin, out upd_begin);
            DateTime upd_end;
            DateTime.TryParse(DateEnd, out upd_end);
            //Открытие подключения к БД и запрос нужных полей с условием даты и времени
            using (var connection = new SqlConnection("Data Source=DESKTOP-STGMEUB;Initial Catalog=DBPractica;Integrated Security=True"))
            {
                connection.Open();
                var com = new SqlCommand($"SELECT \"Дата-время начала (план)\", \"№ задания\", \"№ ПЗ\", \"Толщина (план)\", \"Ширина (план)\", \"Вес (план)\", \"Длина (ПЗ)\", \"Длина (по разметке)\" FROM Practica WHERE \"Дата-время начала (план)\" >= '{upd_begin}' AND \"Дата-время начала (план)\" <='{upd_end}'", connection);
                var reader = com.ExecuteReader();
                //Создание DataTable и загрузка в нее данных из запроса, после чего связка ее со свойством DataSlab
                var dt = new DataTable();
                dt.Load(reader);
                Slyabs = dt;
                //Аналогичные действия для таблицы кратов
                com = new SqlCommand($"select \"№ ПЗ\", \"Длина (по разметке)\", \"Ширина (план)\", \"Вес (план)\", Round(\"Длина (Пз)\"/TRY_CAST(\"Длина (по разметке)\" AS FLOAT),0) AS \"Количество\" FROM Practica as P WHERE \"Дата-время начала (план)\" >= '{upd_begin}' AND \"Дата-время начала (план)\" <='{upd_end}'", connection);
                reader = com.ExecuteReader();
                dt = new DataTable();
                dt.Load(reader);
                Crats = dt;
                //Закрытие подключения и связанных ресурсов
                connection.Close();

            }
        }
    

        public MainWindowViewModel()
        {
            UpdateButton = new LyambdaCommand(UpdateBUttonOnExecuted, UpdateButtonCanExecute);
        }
    }
}
