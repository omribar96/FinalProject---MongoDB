﻿using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FinalProject.Models;

namespace FinalProject
{
    
    public partial class Form1 : Form
    {
        string connectionString = ConfigurationManager.ConnectionStrings["MyMongo"].ConnectionString;
        IMongoCollection<RoomManagement> roomsManagementCollection;

        public Form1()
        {
            InitializeComponent();
        }

        // Upon loading the program
        private void Form1_Load(object sender, EventArgs e)
        {
            // Get the DB Name(As a string)
            MongoDB.Driver.MongoUrl mongoUrl = MongoUrl.Create(connectionString);
            string dbName = mongoUrl.DatabaseName;

            // Create Mongo Client
            MongoDB.Driver.MongoClient mongoClient;

            try
            {
                // we will work on a database in this manner
                mongoClient = new MongoClient(connectionString);

                // Get the DB OBJECT
                IMongoDatabase db = mongoClient.GetDatabase(dbName);

                // Get the Collection 
                // the collection is called "Rooms" - in the first time it will create it
                roomsManagementCollection = db.GetCollection<RoomManagement>("Rooms");

                // When the form is loaded - we would like to get the list of all the rooms
                LoadRoomsUponScreen();

            }
            catch (Exception ex)
            {
                mongoClient = null;
                MessageBox.Show("The following error occured:\n" + ex.Message,
                    "Siesta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Load all the rooms upon the user's screen
        private void LoadRoomsUponScreen()
        {
            // To hide _id by Mongo
            var results = roomsManagementCollection.Find(_ => true)
                .Project(r => new
                {
                    r.RoomFloor,
                    r.RoomNumber,
                    r.RoomType,
                    r.RoomStatus,
                    r.RoomPrice
                })
                .ToList();

            // Clear the text box values
            comboBox_Filter_Room_Floor.SelectedItem = null;
            comboBox_Filter_By_Type.SelectedItem = null;
            comboBox_Filter_By_Status.SelectedItem = null;
            textBox_Filert_By_Price.Clear();



            dataGridView.DataSource = results;
        }

        // Refresh the Data grid
        private void btn_refresh_Click(object sender, EventArgs e)
        {
            LoadRoomsUponScreen();
        }

        // Function to Insert a new room to the system
        private void btn_Insert_Room_Click(object sender, EventArgs e)
        {
            // To Do - Stage 1: Get the information from the screen
            FinalProject.Models.RoomManagement room = GetRoomDetailsFromScreen();

            // To Do - Stage2: Insert the data into the Collection(Into the MongoDB)
            try
            {
                roomsManagementCollection.InsertOne(room);
                MessageBox.Show("The Following product was inserted:\n" + room.ToString(),
                    "Product was inserted",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                // If the insert succeeded - refresh the screen with the new information
                LoadRoomsUponScreen();
            }

            catch (Exception ex)
            {
                MessageBox.Show("The following error occured:\n" + ex.Message,
                   "Siesta",
                   MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }

            textBox_Roon_No.Clear();
            comboBox_Room_Floors.SelectedItem = null;
            comboBox_Room_Type.SelectedItem = null;
            comboBox_Room_Status.SelectedItem = null;
            textBox_Room_Price.Clear();
        }

        // Function to get room details from screen
        private RoomManagement GetRoomDetailsFromScreen()
        {
            int roomNo = Convert.ToInt32(textBox_Roon_No.Text);
            int roomFloor = Convert.ToInt32(comboBox_Room_Floors.SelectedItem);
            int roomType = Convert.ToInt32(comboBox_Room_Type.SelectedItem);
            int roomStatus = Convert.ToInt32(comboBox_Room_Status.SelectedItem);
            double roomPrice = Convert.ToDouble(textBox_Room_Price.Text);

            RoomManagement room = new RoomManagement( roomFloor,  roomNo,  roomType,  roomStatus,  roomPrice);

            return room;
        }

        // Filter by floor
        private void btn_Filter_By_Floor_Click(object sender, EventArgs e)
        {
            // Stage 1: Take the data out of the screen
            int floorToFilter = Convert.ToInt32(comboBox_Filter_Room_Floor.SelectedItem);

            // Stage 2: Build the filter
            FilterDefinition<RoomManagement> filter = Builders<RoomManagement>.Filter.Eq(r => r.RoomFloor, floorToFilter);

            // Stage 3: Perform the filter
            List<RoomManagement> results = roomsManagementCollection.Aggregate().Match(filter).ToList();

            // Show in Data Grid
            dataGridView.DataSource = results;

            comboBox_Filter_Room_Floor.SelectedItem = null;
        }

        // Filter by type
        private void btn_Filter_By_Type_Click(object sender, EventArgs e)
        {
            // Stage 1: Take the data out of the screen
            int typeToFilter = Convert.ToInt32(comboBox_Filter_By_Type.SelectedItem);

            // Stage 2: Build the filter
            FilterDefinition<RoomManagement> filter = Builders<RoomManagement>.Filter.Eq(r => r.RoomType, typeToFilter);

            // Stage 3: Perform the filter
            List<RoomManagement> results = roomsManagementCollection.Aggregate().Match(filter).ToList();

            // Show in Data Grid
            dataGridView.DataSource = results;

            comboBox_Filter_By_Type.SelectedItem = null;
        }

        // Filter by status
        private void btn_Filter_By_Status_Click(object sender, EventArgs e)
        {
            // Stage 1: Take the data out of the screen
            int statusToFilter = Convert.ToInt32(comboBox_Filter_By_Status.SelectedItem);

            // Stage 2: Build the filter
            FilterDefinition<RoomManagement> filter = Builders<RoomManagement>.Filter.Eq(r => r.RoomStatus, statusToFilter);

            // Stage 3: Perform the filter
            List<RoomManagement> results = roomsManagementCollection.Aggregate().Match(filter).ToList();

            // Show in Data Grid
            dataGridView.DataSource = results;

            comboBox_Filter_By_Status.SelectedItem = null;
        }

        // Filter by price
        private void btn_Filter_By_Price_Click(object sender, EventArgs e)
        {
            // Stage 1: Take the data out of the screen
            double priceToFilter = Convert.ToDouble(textBox_Filert_By_Price.Text);

            // Stage 2: Build the filter
            FilterDefinition<RoomManagement> filter = Builders<RoomManagement>.Filter.Lte(r => r.RoomPrice, priceToFilter);

            // Stage 3: Perform the filter
            List<RoomManagement> results = roomsManagementCollection.Aggregate().Match(filter).ToList();

            // Show in Data Grid
            dataGridView.DataSource = results;

            textBox_Filert_By_Price.Clear();
        }

        // Function for Cell double click window open
        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Gets a table
            UDForm udForm = new UDForm(roomsManagementCollection); // roomsCollection


            //udForm.textBox_UD_Mongo_Id.Text = dataGridView.CurrentRow.Cells[0].Value.ToString();
            udForm.textBox_UD_Room_No.Text = dataGridView.CurrentRow.Cells[1].Value.ToString();
            udForm.textBox_UD_Room_Floor.Text = dataGridView.CurrentRow.Cells[0].Value.ToString();
            udForm.comboBox_UD_Room_Type.SelectedItem= dataGridView.CurrentRow.Cells[2].Value.ToString();
            udForm.comboBox_UD_Room_Status.SelectedItem = dataGridView.CurrentRow.Cells[3].Value.ToString();
            udForm.textBox_UD_Room_Price.Text = dataGridView.CurrentRow.Cells[4].Value.ToString();

            // Show the dialog after the fields have been filled
            udForm.ShowDialog(this);

            // To Do - Refresh the screen after we are coming back from the delete/update screen
            LoadRoomsUponScreen();
        }

    }
}
