namespace SadaqaAccounting.Shared.EmailSettings
{
    public static class PreparedEmail
    {
        private static readonly Dictionary<string, IEmailBodyGenerator> Generators = new(StringComparer.OrdinalIgnoreCase)
        {
            #region Accounting Module
            ["Budget"] = new AccountBudgetApplicationBodyGenerator(),
            #endregion
        };

        public static string GetEmailBody(ApplicationInformation info)
        {
            if (!Generators.TryGetValue(info.FeatureName, out var generator))
                throw new NotSupportedException($"Email not supported for: {info.FeatureName}");

            var featureHtml = generator.GenerateBody(info);

            return $@"
            <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Arial, sans-serif;
                            color: #333;
                            padding: 20px;
                        }}
                        .details-table {{
                            width: 100%;
                            border-collapse: collapse;
                            margin-top: 15px;
                            font-size: 14px;
                        }}
                        .details-table th {{
                            background: #28a745;
                            color: #ffffff;
                            text-align: left;
                            padding: 8px;
                            border: 1px solid;
                        }}
                        .details-table td {{
                            border: 1px solid #ccc;
                            padding: 8px;
                            vertical-align: top;
                        }}
                        .info {{
                            margin-bottom: 10px;
                            line-height: 0.5;
                        }}
                        .btn {{
                            display: inline-block;
                            padding: 12px 24px;
                            font-size: 14px;
                            font-weight: bold;
                            color: #ffffff;
                            background-color: #28a745;
                            text-decoration: none;
                            border-radius: 4px;
                            margin: 20px 0;
                        }}
                        .footer-note {{
                            font-size: 12px;
                            color: #777;
                            font-style: italic;
                            margin-top: 20px;
                            border-top: 1px solid #ddd;
                            padding-top: 10px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='info'>
                        <p><strong>Department:</strong> {info.Department}</p>
                        <p><strong>Designation:</strong> {info.Designation}</p>
                        <p><strong>Employee Name:</strong> {info.Employee}</p>
                        <p><strong>Joining Date:</strong> {info.JoiningDate:dd MMM yyyy}</p>
                    </div>
                    <p><strong>Application Details:</strong></p>
                    {featureHtml}
                    <p>You can review and take action on the request by clicking the button below:</p>
                    <a class='btn' href='{info.ApprovalLink}' target='_blank'>
                        <img src='https://img.icons8.com/ios-filled/16/ffffff/visible.png' alt='View Icon' style='vertical-align:middle; margin-right:6px;' />
                        View
                    </a>
                    <div class='footer-note'>
                        This is a system-generated email. Please do not reply to this message.
                    </div>
                </body>
            </html>";
        }

        public static string GetRFQEmailBody(string? vendorName, DateTime? dueDate, string? companyName)
        {
            return $@"
                    <html>
                      <body style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                        <p>Dear {vendorName},</p>

                        <p>We hope this message finds you well.</p>

                        <p>
                          We are pleased to invite you to submit a quotation for the supply of items
                          listed in the attached document.
                        </p>

                        <p>When preparing your quotation, kindly include the following details:</p>
                        <ul>
                          <li>Price</li>
                          <li>Lead time (in days)</li>
                          <li>Offered quantity</li>
                          <li>Validity period of your quotation</li>
                          <li>Any additional terms and conditions</li>
                        </ul>

                        <p>
                          We kindly request you to submit your quotation on or before
                          <b>{dueDate:dd MMM yyyy}</b>.
                        </p>

                        <p>
                          If you require any further clarification or additional information,
                          please do not hesitate to contact us.
                        </p>

                        <p>We look forward to receiving your quotation.</p>

                        <p style='margin-top:20px;'>
                          Best regards,<br />
                          <b>{companyName}</b><br />
                        </p>
                      </body>
                    </html>";
        }
    }
}