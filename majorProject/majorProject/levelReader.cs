﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Regex ftw
using System.Text.RegularExpressions;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace majorProject
{
    class LevelReader
    {
        private ArrayList levelFiles = new ArrayList();

        //public globals
        public string background;               //background stored as string to load in Load in Game1.cs
        public string levelSong;                //song stored as string to load in Game1.cs
        public ArrayList enemyList;

        int currentLevel;

        public LevelReader()
        {
            this.currentLevel = 0;
            // Get working directory and populate file array
            string workingDirectory = Directory.GetCurrentDirectory();
            string[] files = Directory.GetFiles(workingDirectory);

            // set regular expression to check for only files that end in '.lvl'
            Regex reg = new Regex("$.lvl");

            // check all files in the working directory to see if any end with .lvl,
            // if they do, add them to the list of level files
            foreach (string file in files)
            {
                if (reg.IsMatch(file))
                {
                    levelFiles.Add(file);
                }
            }

            // load items for first level
            getNextLevel();
        }

        /// <summary>
        /// Gets the next level
        /// </summary>
        /// <returns></returns>
        public void getNextLevel()
        {
            // get current level from the ArrayList of levels and increment the currentLevel
            string level = (string)levelFiles[currentLevel];
            currentLevel++;

            // Create reader from current level
            StreamReader reader = new StreamReader(File.OpenRead(level));

            // Read the first line, and store it as the background
            this.background = reader.ReadLine();

            // Read the second line, and store it as the song
            this.levelSong = reader.ReadLine();
 
            // The rest of the lines will be enemy placements/appear times
            string line = reader.ReadLine();
            while (line != null)
            {
                //TODO: figure out how to concatonate strings
                //First entry will be the enemy type
                string type = "";

                //Second entry will be the time;
                string time = "";
                //TODO: extract number from string
                int appearTime = 0;

                //Third will the the appear x
                string X = "";
                //TODO: extract number from string
                int xpos = 0;

                if (type == "Grunt")
                {
                    //Create a blank grunt
                    Grunt grunt = new Grunt();
                    grunt.xPos = 0;
                    grunt.appearTime = appearTime;
                }
                /*
                 * Additional types go here
                else if
                {
                }
                 */
                else
                {
                    // Do nothing and print error
                    Console.Error.WriteLine("Error unrecognized enemy type: <" + type + ">");
                }
            }
        }
    }
}