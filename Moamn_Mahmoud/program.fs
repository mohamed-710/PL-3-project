let writeToFile (userInfo: UserInfo) score =
    
    let solutionDirectory = _SOURCE_DIRECTORY_
    let filePath = Path.Combine(solutionDirectory, "quiz_results.txt")

    let content = sprintf "Name: %s\nID: %s\nScore: %d\n\n" userInfo.Name userInfo.ID score
    try
        File.AppendAllText(filePath, content)  
        MessageBox.Show("Your results have been saved.") |> ignore
    with
    | ex -> MessageBox.Show(sprintf "Error saving the results: %s" ex.Message) |> ignore