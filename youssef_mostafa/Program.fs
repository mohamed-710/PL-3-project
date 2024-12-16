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
                | _ -> ()
    )