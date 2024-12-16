open System
open System.Drawing
open System.Windows.Forms
open System.Collections.Generic
open System.IO

// Define user information and questions
type UserInfo = { Name: string; ID: string }
type Question = {
    Text: string
    Options: string list option
    CorrectAnswer: string
    QuestionType: string
    Score: int
}

type QuizState = { UserAnswers: Map<int, string>; CurrentIndex: int; TimeLeft: int }

let initialState = { UserAnswers = Map.empty<int, string>; CurrentIndex = 0; TimeLeft = 300 }  // Timer starts at 5 minutes (300 seconds)

// Questions for the quiz
let questions =
    Map.ofList [
        0, { Text = "What is the capital of France?"; Options = Some ["Paris"; "London"; "Berlin"; "Madrid"]; CorrectAnswer = "Paris"; QuestionType = "MultipleChoice"; Score = 1 }
        1, { Text = "Solve: 5 + 3 = ?"; Options = None; CorrectAnswer = "8"; QuestionType = "Written"; Score = 2 }
        2, { Text = "Which language is functional?"; Options = Some ["F#"; "Python"; "Java"; "C#"]; CorrectAnswer = "F#"; QuestionType = "MultipleChoice"; Score = 1 }
        3, { Text = "What is your favorite programming language?"; Options = None; CorrectAnswer = "F#"; QuestionType = "Written"; Score = 2 }
        4, { Text = "Explain the concept of Polymorphism in OOP."; Options = None; CorrectAnswer = "Polymorphism allows objects of different classes to be treated as objects of a common superclass. It is one of the key features of Object-Oriented Programming."; QuestionType = "Written"; Score = 3 }
        5, { Text = "Describe the difference between a stack and a queue."; Options = None; CorrectAnswer = "A stack follows Last In, First Out (LIFO), while a queue follows First In, First Out (FIFO)."; QuestionType = "Written"; Score = 3 }
        6, { Text = "Which of the following is a functional programming language?"; Options = Some ["F#"; "Java"; "C#"; "Ruby"]; CorrectAnswer = "F#"; QuestionType = "MultipleChoice"; Score = 1 }
        7, { Text = "Which of these is not an OOP concept?"; Options = Some ["Inheritance"; "Encapsulation"; "Recursion"; "Abstraction"]; CorrectAnswer = "Recursion"; QuestionType = "MultipleChoice"; Score = 1 }
    ]

// Function to calculate the score
let calculateScore userAnswers =
    questions
    |> Map.fold (fun acc index question -> 
        match userAnswers |> Map.tryFind index with
        | Some userAnswer when userAnswer = question.CorrectAnswer -> acc + question.Score
        | _ -> acc
    ) 0

