/** 
 *  "Fill In The Blank"
 *  
 *  A word game programmed by Jacob Hunt
 *  (jacobhuntemail@gmail.com).
 *  
 *  Program selects a random story from a directory of text files and uses it
 *  to construct a round of the game.  Blank words in the text file are 
 *  contained within curly braces.  Example:
 *  
 *      "Roses are {color}, {plural noun} are {color}"
 *  
 *  The game prompts the user for input to fill in the unknown words and then
 *  outputs the resulting story for the sake of humor.
 *  
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Fill_In_The_Blank
{
    class Program
    {
        // Directory in which text files of stories exist
        const string STORY_DIRECTORY = "../../../stories/";

        // Maximum width of a line of output text (in units of number-of-chars)
        const int LINE_WIDTH = 70;

        /** Main Method */
        static void Main(string[] args)
        {
            GameControl gameControl = new GameControl(STORY_DIRECTORY);
            gameControl.promptPlayerForInput();
            gameControl.storyBook.editStory(gameControl.playerResponses);
            Console.Write('\n');
            HelperMethods.consolePrintWordWrap(gameControl.storyBook.currentStory, LINE_WIDTH);
        }
    }


    /** Class to manage the details of game/program execution */
    class GameControl
    {
        // Object to contain the stories to be used in the game
        public StoryBook storyBook;

        // List of player input for blank spaces in story
        public List<string> playerResponses;


        /** Constructor */
        public GameControl(string storyDirectory)
        {
            // Initialize objects
            this.storyBook = new StoryBook(storyDirectory);
            this.playerResponses = new List<string>();
        }


        /** Prompt the player for input (words/phrases to fill in the blank
         *  spaces in the story). */
        public void promptPlayerForInput()
        {
            foreach (string s in storyBook.blankWords)
            {
                // Prompt user
                Console.Write("Enter a " + s + ": ");

                // Add response to "playerResponses" list
                this.playerResponses.Add(Console.ReadLine());
            }
        }
    }


    /** Class to contain the diferent Mid Libs stories */
    class StoryBook
    {
        // Array of file paths for the text files containing stories
        private string[] textFiles;

        // String which contains the raw text for the current story
        public string currentStory;

        // Array which contains the blank words in the current story, with the
        // string values indicating the blank's classification (noun, verb, "a
        // kind of animal", etc.)
        public List<string> blankWords;


        /** Constructor */
        public StoryBook(string storyDirectory)
        {
            try
            {
                // Initialize and set instance variables
                textFiles = Directory.GetFiles(storyDirectory);
                this.blankWords = new List<string>();
                loadNewStory();
            }
            catch (Exception e)
            {
                // Print error message to console if an error occurs
                Console.Write(e.Message);
            }
        }

        /** Returns a StringBuilder object from the contents of a text file from
            the stories directory */
        public string getRandomStory()
        {
            // Get the file path of a random file from the stories directory
            string filePath = this.textFiles[
                new Random().Next(0, this.textFiles.Length)
            ];

            // Return a StringBuilder of the file contents
            return new string(new StreamReader(filePath).ReadToEnd());
        }


        // Sets the value of "currentStory" to a random story from the
        // stories directory and fills the blank word list
        public void loadNewStory()
        {
            this.currentStory = getRandomStory();
            this.setBlankWordList();
        }


        /** Sets the value blankWords array based on the contents of
         * "currentStory" */
        public void setBlankWordList()
        {
            // Boolean to indicate whether or not the method is currently
            // reading a blank word
            bool isBlankWordBeingProcessed = false;

            // StringBuilder to contain the blank word currently being read
            StringBuilder currentBlank = new StringBuilder();

            // Iterate through the currentStory string, adding blank fields to
            // the blankWords list as they are encountered
            foreach (char c in this.currentStory)
            {
                if (c == '}')
                {
                    isBlankWordBeingProcessed = false;
                    blankWords.Add(currentBlank.ToString());
                    currentBlank.Clear();
                }

                if (isBlankWordBeingProcessed)
                {
                    currentBlank.Append(c);
                }

                if (c == '{')
                {
                    isBlankWordBeingProcessed = true;
                }

            }

        }


        /** Edits the current story by filling in the blank spaces with input
         *  provided by player. */
        public void editStory(List<string> playerResponses)
        {

            // A StringBuilder copy of the story string which allows for the
            // story to be edited
            StringBuilder storyWorkingCopy = new StringBuilder(this.currentStory);

            // Boolean to indicate whether or not the method is currently
            // editing a blankspace
            bool isBlankBeingProcessed = false;

            // Counter to keep track of which word is currently being replaced
            int numWordsReplaced = 0;

            // Iterate through the currentStory string, adding blank fields to
            // the blankWords list as they are encountered
            for (int i = 0; i < storyWorkingCopy.Length; i++)
            {
                if (storyWorkingCopy[i] == '}')
                {
                    isBlankBeingProcessed = false;
                    i--;
                    storyWorkingCopy.Remove(i, 2);
                    storyWorkingCopy.Insert(i, playerResponses[numWordsReplaced]);
                    numWordsReplaced++;
                }

                if (isBlankBeingProcessed)
                {
                    // Remove characters
                    storyWorkingCopy.Remove(i, 1);
                    i--;
                }

                if (storyWorkingCopy[i] == '{')
                {
                    isBlankBeingProcessed = true;
                }

            }

            // Replace "currentStory" string with edited story
            this.currentStory = storyWorkingCopy.ToString();

        }
    }


    /** A class to contain useful methods that don't fit into any particular
     *  categotry */
    class HelperMethods
    {
        /** Print a string to the console using a "word-wrap" format.  Line 
         *  width will not exceed the value of 'maxWidth' (units are in 'number
         *  of characters'). */
        public static void consolePrintWordWrap(string inputString, int maxWidth)
        {
            StringBuilder workingStringCopy = new StringBuilder(inputString);

            int mostRecentWhitespaceIndex = 0;
            int currentColumn = 0;

            for (int i = 1; i < workingStringCopy.Length; i++)
            {
                currentColumn++;

                if (Char.IsWhiteSpace(workingStringCopy[i]))
                {
                    mostRecentWhitespaceIndex = i;
                }

                if (workingStringCopy[i - 1] == '\n')
                {
                    currentColumn = 0;
                }

                if (currentColumn % maxWidth == 0 && currentColumn > 0)
                {
                    workingStringCopy.Remove(mostRecentWhitespaceIndex, 1);
                    workingStringCopy.Insert(mostRecentWhitespaceIndex, '\n');
                    currentColumn = 0;
                }
            }

            Console.WriteLine(workingStringCopy);
        }
    }
}
