namespace EventBookingWeb.Helpers
{
    public static class EmailHelper
    {
        public static string GetEmailTemplate(string title, string body)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{
            font-family: Arial, sans-serif;
            line-height: 1.6;
            color: #333;
        }}
        .container {{
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
        }}
        .header {{
            background-color: #4CAF50;
            color: white;
            padding: 20px;
            text-align: center;
        }}
        .content {{
            padding: 20px;
            background-color: #f9f9f9;
        }}
        .footer {{
            padding: 20px;
            text-align: center;
            font-size: 12px;
            color: #666;
        }}
        .button {{
            display: inline-block;
            padding: 10px 20px;
            background-color: #4CAF50;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin: 10px 0;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>{title}</h1>
        </div>
        <div class='content'>
            {body}
        </div>
        <div class='footer'>
            <p>© 2024 Event Booking System. All rights reserved.</p>
            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}

