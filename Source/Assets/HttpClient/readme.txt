Http Client (v1.03)
----------------------------

Thank you for downloading Http Client we hope you enjoy using it!

A demo scene is included that shows how to use most features
Full documentation can be found on the website or if you need additional support please contact us at the address below
If you would like to see any features added please get in touch

Support Website: http://www.claytoninds.com/
Support Email: clayton.inds+support@gmail.com

------------------------------------------------------------------------------------------------------------------------
Why not check out our other plugins:
- Quick Save (Saving made easy)
- Task Parallel (Threading made easy)
- Windows Store Native (Windows UWP integration made easy)

------------------------------------------------------------------------------------------------------------------------
When building for mono based platforms and using HTTPS, you may encounter certificate exceptions as mono does not include root certificates by default.
You can add the follow code to automatically accept all certificates or customise it to add your own validation.

System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
{
    return true;
};

------------------------------------------------------------------------------------------------------------------------

Methods:
HttpClient.Abort
HttpClient.Delete
HttpClient.GetString
HttpClient.GetByteArray
HttpClient.Patch
HttpClient.Post
HttpClient.Put