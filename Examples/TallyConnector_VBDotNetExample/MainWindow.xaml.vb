Imports TallyConnector.Services

Class MainWindow
    Dim _tallyService As New TallyConnector.Services.TallyService
    Public Sub New()
        'Setting up address
        _tallyService.Setup("localhost", 9000)
    End Sub

    Private Async Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        Dim masterTypeStats = Await _tallyService.GetMasterStatisticsAsync()
        MstStats.ItemsSource = masterTypeStats
        Dim reqOptions As New TallyConnector.Core.Models.RequestOptions
        reqOptions.ToDate = TallyService.GetToDate()
        Dim VchTypeStats = Await _tallyService.GetVoucherStatisticsAsync(reqOptions)
        Vchtats.ItemsSource = VchTypeStats
    End Sub
End Class
