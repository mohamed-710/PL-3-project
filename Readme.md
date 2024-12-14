# Quiz Application

This project is a Windows Forms-based quiz application built using F#. The application allows users to:

- Login with their Name and ID.
- Attempt multiple-choice and written questions.
- View a timer for the quiz.
- Navigate between questions using "Next" and "Back" buttons.
- Submit the quiz and view their final score.
- Save their results in a file.

## Features

1. **User Login:**
   - Users must provide their Name and ID to start the quiz.

2. **Question Types:**
   - Multiple-choice questions with radio button options.
   - Written questions where users can type their answers.

3. **Timer:**
   - A countdown timer for the quiz.
   - Automatically submits the quiz when time is up.

4. **Navigation:**
   - Navigate between questions using "Next" and "Back" buttons.

5. **Score Calculation:**
   - Calculates the user's score based on correct answers.

6. **Results Storage:**
   - Saves user results to a text file for record-keeping.

## How to Run

1. Clone the repository or download the source code.
2. Open the project in an F#-compatible IDE like Visual Studio or Rider.
3. Build and run the project.
4. Provide a valid Name and ID to start the quiz.
## Dependencies

- .NET Framework (Windows Forms)
- F#
## Results Storage

User results are stored in the `Results` directory as a text file. Each entry includes:
- User's Name
- User's ID
- Final Score

