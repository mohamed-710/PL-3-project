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
    form.ShowDialog() |> ignore