open System
open System.Drawing
open System.Windows.Forms
open System.Collections.Generic
open System.IO


type UserInfo = { Name: string; ID: string }
type Question = {
    Text: string
    Options: string list option
    CorrectAnswer: string
    QuestionType: string
    Score: int
}

type QuizState = { UserAnswers: Map<int, string>; CurrentIndex: int; TimeLeft: int }

let initialState = { UserAnswers = Map.empty<int, string>; CurrentIndex = 0; TimeLeft = 300 }  


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


let calculateScore userAnswers =
    questions
    |> Map.fold (fun acc index question -> 
        match userAnswers |> Map.tryFind index with
        | Some userAnswer when userAnswer = question.CorrectAnswer -> acc + question.Score
        | _ -> acc
    ) 0


let generateQuizResult userInfo userAnswers =
    let score = calculateScore userAnswers
    let answersMessage =
        questions
        |> Map.fold (fun acc index question -> 
            let userAnswer = userAnswers |> Map.tryFind index |> Option.defaultValue "No Answer"
            let correctAnswer = question.CorrectAnswer
            acc + sprintf "Q%d: %s\nYour Answer: %s\nCorrect Answer: %s\n\n" (index + 1) question.Text userAnswer correctAnswer
        ) ""
    sprintf "Name: %s\nID: %s\nYour Final Score: %d\n\n%s" userInfo.Name userInfo.ID score answersMessage


let writeToFile (userInfo: UserInfo) score =
    
    let solutionDirectory = __SOURCE_DIRECTORY__
    let filePath = Path.Combine(solutionDirectory, "quiz_results.txt")

    let content = sprintf "Name: %s\nID: %s\nScore: %d\n\n" userInfo.Name userInfo.ID score
    try
        File.AppendAllText(filePath, content)  
        MessageBox.Show("Your results have been saved.") |> ignore
    with
    | ex -> MessageBox.Show(sprintf "Error saving the results: %s" ex.Message) |> ignore


let createQuizForm (userInfo: UserInfo) (questions: Map<int, Question>) =
    let form = new Form(Text = sprintf "Quiz - %s" userInfo.Name, Size = Size(500, 500), StartPosition = FormStartPosition.CenterScreen)

    let mutable currentState = initialState

    let questionNumberLabel = new Label(AutoSize = true, Location = Point(350, 10), Font = new Font("Arial", 10.0f, FontStyle.Regular), ForeColor = Color.DarkGray)
    let questionLabel = new Label(Location = Point(20, 50), AutoSize = true, Font = new Font("Arial", 12.0f))
    let optionsPanel = new FlowLayoutPanel(Location = Point(20, 100), Size = Size(460, 100),FlowDirection=FlowDirection.TopDown)
    let answerTextBox = new TextBox(Location = Point(20, 220), Size = Size(200, 30))
    let nextButton = new Button(Text = "Next", Location = Point(250, 220), BackColor = Color.LightBlue)
    let backButton = new Button(Text = "Back", Location = Point(340, 220), BackColor = Color.LightGray)
    let finishButton = new Button(Text = "Finish", Location = Point(430, 220), BackColor = Color.LightGreen)

    
    let timerLabel = new Label(Text = "Time Left: 5:00", Location = Point(20, 10), AutoSize = true, Font = new Font("Arial", 10.0f, FontStyle.Regular), ForeColor = Color.DarkRed)

    
    let timer = new Timer(Interval = 1000)  
    timer.Start()

    
    timer.Tick.Add(fun _ -> 
        if currentState.TimeLeft > 0 then
            currentState <- { currentState with TimeLeft = currentState.TimeLeft - 1 }
            let minutes = currentState.TimeLeft / 60
            let seconds = currentState.TimeLeft % 60
            timerLabel.Text <- sprintf "Time Left: %02d:%02d" minutes seconds
        else
            timer.Stop()
            MessageBox.Show("Time's up!") |> ignore
            form.Close()
    )

    let displayQuestion index =
        if questions.ContainsKey(index) then
            let question = questions.[index]
            questionNumberLabel.Text <- sprintf "Question %d of %d" (index + 1) questions.Count
            questionLabel.Text <- sprintf "%s (Score: %d)" question.Text question.Score
            optionsPanel.Controls.Clear()

            match question.QuestionType with
            | "MultipleChoice" -> 
                answerTextBox.Visible <- false
                question.Options
                |> Option.iter (fun options -> 
                    options |> List.iter (fun option -> 
                        let radioButton = new RadioButton(Text = option, AutoSize = true)
                        optionsPanel.Controls.Add(radioButton)))
            | "Written" -> answerTextBox.Visible <- true
            | _ -> answerTextBox.Visible <- false

    nextButton.Click.Add(fun _ -> 
        if questions.ContainsKey(currentState.CurrentIndex) then
            let question = questions.[currentState.CurrentIndex]

            match question.QuestionType with
            | "MultipleChoice" -> 
                let selectedOption =
                    optionsPanel.Controls
                    |> Seq.cast<Control>
                    |> Seq.tryFind (fun c -> (c :?> RadioButton).Checked)
                    |> Option.map (fun c -> c.Text)

                currentState <- { currentState with UserAnswers = currentState.UserAnswers.Add(currentState.CurrentIndex, selectedOption |> Option.defaultValue "") }

            | "Written" -> 
                currentState <- { currentState with UserAnswers = currentState.UserAnswers.Add(currentState.CurrentIndex, answerTextBox.Text.Trim()) }

            | _ -> ()

            if questions.ContainsKey(currentState.CurrentIndex + 1) then
                currentState <- { currentState with CurrentIndex = currentState.CurrentIndex + 1 }
                displayQuestion currentState.CurrentIndex

                match questions.[currentState.CurrentIndex].QuestionType with
                | "Written" -> answerTextBox.Text <- currentState.UserAnswers |> Map.tryFind currentState.CurrentIndex |> Option.defaultValue ""
                | "MultipleChoice" -> 
                    let savedAnswer = currentState.UserAnswers |> Map.tryFind currentState.CurrentIndex |> Option.defaultValue ""
                    optionsPanel.Controls
                    |> Seq.cast<Control>
                    |> Seq.iter (fun c -> 
                        let radioButton = c :?> RadioButton
                        radioButton.Checked <- (radioButton.Text = savedAnswer))
                | _ -> ()
    )

    backButton.Click.Add(fun _ -> 
        if currentState.CurrentIndex > 0 then
            currentState <- { currentState with CurrentIndex = currentState.CurrentIndex - 1 }
            displayQuestion currentState.CurrentIndex

            match questions.[currentState.CurrentIndex].QuestionType with
            | "Written" -> 
                answerTextBox.Text <- currentState.UserAnswers |> Map.tryFind currentState.CurrentIndex |> Option.defaultValue ""
            | "MultipleChoice" -> 
                let savedAnswer = currentState.UserAnswers |> Map.tryFind currentState.CurrentIndex |> Option.defaultValue ""
                optionsPanel.Controls
                |> Seq.cast<Control>
                |> Seq.iter (fun c -> 
                    let radioButton = c :?> RadioButton
                    radioButton.Checked <- (radioButton.Text = savedAnswer))
            | _ -> ()
    )

    finishButton.Click.Add(fun _ -> 
        let score = calculateScore currentState.UserAnswers
        writeToFile userInfo score  
        let resultMessage = generateQuizResult userInfo currentState.UserAnswers
        MessageBox.Show(resultMessage) |> ignore
        form.Close())

    form.Controls.AddRange([| questionNumberLabel; questionLabel; optionsPanel; answerTextBox; nextButton; backButton; finishButton; timerLabel |])
    displayQuestion currentState.CurrentIndex
    form.ShowDialog() |> ignore


let usedIds = HashSet<string>()


let createMainForm () =
    let form = new Form(Text = "Quiz Application", Size = Size(500, 450), StartPosition = FormStartPosition.CenterScreen)
    let login = new Label(Text = "Quiz" , Location = Point(210, 20), AutoSize = true, Font = new Font("Arial", 15.0f))
    login.ForeColor <- Color.FromArgb(255, 0, 0)
    let nameLabel = new Label(Text = "Name:", Location = Point(20, 100), AutoSize = true, Font = new Font("Arial", 12.0f))
    let nameTextBox = new TextBox(Location = Point(120, 100), Size = Size(300, 30))
    let idLabel = new Label(Text = "ID:", Location = Point(20, 150), AutoSize = true, Font = new Font("Arial", 12.0f))
    let idTextBox = new TextBox(Location = Point(120, 150), Size = Size(300, 30))
    let startButton = new Button(Text = "Start Quiz", AutoSize = true, Location = Point(200, 200), BackColor = Color.LightBlue)

    startButton.Click.Add(fun _ -> 
        let name = nameTextBox.Text.Trim()
        let id = idTextBox.Text.Trim()

        if name = "" || id = "" then
            MessageBox.Show("Please fill in all fields!") |> ignore
        elif usedIds.Contains(id) then
            MessageBox.Show("This ID has already been used. Please use a different ID.") |> ignore
        else
            usedIds.Add(id) |> ignore
            let userInfo = { Name = name; ID = id }
            createQuizForm userInfo questions
    )

    form.Controls.AddRange([| login; nameLabel; nameTextBox; idLabel; idTextBox; startButton |])
    Application.Run(form)

createMainForm()
