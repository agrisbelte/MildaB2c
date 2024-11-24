// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Milda.B2C.Maui.Platforms.Android
{
    [Activity(Exported =true)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msal[REPLACE THIS WITH THE CLIENT ID OF YOUR APP]")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}
