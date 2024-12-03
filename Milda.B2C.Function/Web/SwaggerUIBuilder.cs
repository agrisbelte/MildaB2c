namespace Milda.B2C.Function.Web;

public static class SwaggerUIBuilder
{
    public static string Build(string swaggerJsonEndpoint)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Swagger UI</title>
                <link rel=""stylesheet"" href=""https://cdnjs.cloudflare.com/ajax/libs/swagger-ui/4.15.5/swagger-ui.min.css"" />
            </head>
            <body>
                <div id=""swagger-ui""></div>
                <script src=""https://cdnjs.cloudflare.com/ajax/libs/swagger-ui/4.15.5/swagger-ui-bundle.min.js""></script>
                <script>
                    window.onload = () => {{
                        const ui = SwaggerUIBundle({{
                            url: '{swaggerJsonEndpoint}',
                            dom_id: '#swagger-ui'
                        }});
                    }};
                </script>
            </body>
            </html>
        ";
    }
}