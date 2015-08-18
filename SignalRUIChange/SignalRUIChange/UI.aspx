<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UI.aspx.cs" Inherits="SignalRUIChange.UI" %>

<!DOCTYPE html>
<html>
<head>
    <script src="Scripts/jquery-2.0.2.min.js"></script>
    <script src="Scripts/jquery-ui-1.10.0.js"></script>
    <script src="Scripts/jquery.signalR-1.0.0.js"></script>
    <script src="/signalr/hubs"></script>

    <script type="text/javascript">
        $(function () {
            var moveScooby = $.connection.moveScoobyHub;

            moveScooby.client.runScoobyForward = function () {
                $("#scooby").animate({
                    marginLeft: '+=1'
                }, 0);
            }

            moveScooby.client.runScoobyBackward = function () {
                $("#scooby").animate({
                    marginLeft: '-=1'
                }, 0);
            }

            moveScooby.client.runScoobyUpward = function () {
                $("#scooby").animate({
                    marginTop: '-=30'
                }, 500, 'easeOutCubic', function () {
                    $("#scooby").animate({
                        marginTop: '+=30'
                    }, 500, 'easeInExpo');
                });
            }

            $.connection.hub.start().done(function () {
                $(document).keydown(function (e) {
                    switch (e.which) {
                        case 39:
                            moveScooby.server.moveScoobyForward();
                            break;
                        case 37:
                            moveScooby.server.moveScoobyBackward();
                            break;
                        case 32:
                            moveScooby.server.moveScoobyUpward();
                            break;
                    }
                });
            }
            );
        });
    </script>
</head>
<body>
    <div style="background-image: url('Images/scooby01.jpg'); background-repeat: no-repeat; height: 438px; width: 584px">
        <img src="Images/aniscooby.gif" id="scooby" width="100" height="100" style="margin-top: 340px;" />
    </div>
</body>
</html>
