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
