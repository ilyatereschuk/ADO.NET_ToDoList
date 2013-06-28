using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data.SqlClient;

namespace ADO_ToDoList
{
    public class TaskEntity : INotifyPropertyChanged
    {
        int id;
        int priority;
        String title, description;
        DateTime startDate, dueDate;
        Boolean isCompleted;
        //Отслеживает, не изменилась ли запись
        Boolean isCommitNeeded = false;


        #region TaskProperties

        public Boolean IsCommitNeeded
        {
            get { return isCommitNeeded; }
            set { isCommitNeeded = value;

                OnPropertyChanged("IsCommitNeeded");
            }
        }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public int Priority
        {
            get { return priority; }
            set { priority = value;
                IsCommitNeeded = true;
                OnPropertyChanged("PriorityForUser");
            }
        }
        public String Title
        {
            get { return title; }
            set { title = value;
                IsCommitNeeded = true;
                OnPropertyChanged("Title");
            }
        }
        public String Description
        {
            get { return description; }
            set { description = value;
                IsCommitNeeded = true;
                OnPropertyChanged("Description");
            }
        }
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value;
                IsCommitNeeded = true;
                OnPropertyChanged("StartDate");
            }
        }
        public DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = value;
                IsCommitNeeded = true;
                OnPropertyChanged("DueDate");
            }
        }
        public Boolean IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value;
            IsCommitNeeded = true;
                OnPropertyChanged("IsCompletedForUser");
            }
        }
        public String IsCompletedForUser
        {
            get
            {
                if (isCompleted) return "Выполнена";
                else return "В действии";
            }
        }
        public String PriorityForUser
        {
            get
            {
                switch (priority)
                {
                    case 0: return "Низкий";
                    case 1: return "Обычный";
                    case 2: return "Важный";
                    case 3: return "Очень важный";
                    case 4: return "Критический";
                    default: return "Не задан";
                }
            }
        }
        #endregion TaskProperties


        public TaskEntity(String title, String description,
            DateTime startDate, DateTime dueDate, int priority, Boolean isCompleted, int id = 0)
        {
            this.title = title;
            this.description = description;
            this.startDate = startDate;
            this.dueDate = dueDate;
            this.priority = priority;
            this.isCompleted = isCompleted;
            this.id = id;
        }

        public void RemoveFromTable(String connectionString, String table)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand removeRecord = new SqlCommand("DELETE FROM " + table + " WHERE ID=@id", sqlConnection);
                removeRecord.Parameters.AddWithValue("id", id);
                sqlConnection.Open();
                removeRecord.ExecuteNonQuery();
            }
        }

        public void UpdateInTable(String connectionString, String table)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand updateRecord = new SqlCommand(
                    "UPDATE [dbo]." + table +
                    " SET Title=@title, Description=@description, StartDate=@startDate, "+
                    "DueDate=@dueDate, Priority=@priority, IsCompleted=@isCompleted " +
                    "WHERE ID=@id",
                    sqlConnection);
                updateRecord.Parameters.AddWithValue("id", id);
                updateRecord.Parameters.AddWithValue("title", title);
                updateRecord.Parameters.AddWithValue("description", description);
                updateRecord.Parameters.AddWithValue("startDate", startDate);
                updateRecord.Parameters.AddWithValue("dueDate", dueDate);
                updateRecord.Parameters.AddWithValue("priority", priority);
                updateRecord.Parameters.AddWithValue("isCompleted", isCompleted);
                sqlConnection.Open();
               updateRecord.ExecuteScalar();
            }
        }

        public int AddToTable(String connectionString, String table)
        {
            int resultingId;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand addRecord = new SqlCommand(
                    "INSERT INTO [dbo]." + table +
                    "(Title,Description,StartDate,DueDate,Priority,IsCompleted) " +
                    "VALUES(@title,@description,@startDate,@dueDate,@priority,@isCompleted); SELECT SCOPE_IDENTITY();",
                    sqlConnection);
                addRecord.Parameters.AddWithValue("title", title);
                addRecord.Parameters.AddWithValue("description", description);
                addRecord.Parameters.AddWithValue("startDate", startDate);
                addRecord.Parameters.AddWithValue("dueDate", dueDate);
                addRecord.Parameters.AddWithValue("priority", priority);
                addRecord.Parameters.AddWithValue("isCompleted", isCompleted);
                sqlConnection.Open();
                resultingId = Convert.ToInt32(addRecord.ExecuteScalar());
            }
            return resultingId;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
            
        }
    }
    
    class Model : INotifyPropertyChanged
    {
        UInt16 minPriority,
            maxPriority;
        
        ObservableCollection<TaskEntity> taskEntities = null;
        String connectionStr = "Addr=(local);Database=ToDoListDb;Trusted_Connection=sspi;User ID=Admin";
        String sqlTable = "ToDoList";

        public UInt16 MinPriority
        {
            get { return minPriority; }
        }

        public UInt16 MaxPriority
        {
            get { return maxPriority; }
        }

        public ObservableCollection<TaskEntity> TaskEntities
        {
            get { return taskEntities; }
            set { taskEntities = value; OnPropertyChanged("TaskEntities"); }
        }

        public String ConnectionStr
        {
            get { return connectionStr; }
        }

        public String SqlTable
        {
            get { return sqlTable; }
        }

        public void InitializeTaskEntitiesCollection()
        {
            if (taskEntities != null) taskEntities.Clear();
            using (SqlConnection sqlConnection = new SqlConnection(connectionStr))
            {
                SqlCommand readTable = new SqlCommand("SELECT * FROM [dbo]." + sqlTable, sqlConnection);
                sqlConnection.Open();
                SqlDataReader sqlRdr = readTable.ExecuteReader();
                while (sqlRdr.Read())
                {
                    taskEntities.Add(new TaskEntity(
                        sqlRdr["Title"] as String,
                        sqlRdr["Description"] as String,
                        (DateTime)sqlRdr["StartDate"],
                        (DateTime)sqlRdr["DueDate"],
                        (int)sqlRdr["Priority"],
                        (bool)sqlRdr["IsCompleted"],
                        (int)sqlRdr["ID"]
                        )
                    );
                }
            }
        }

        public Model()
        {
            taskEntities = new ObservableCollection<TaskEntity>();
            InitializeTaskEntitiesCollection();
            minPriority = 0;
            maxPriority = 4;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    
}
