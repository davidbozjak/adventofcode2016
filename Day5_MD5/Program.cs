using System.Text;

string doorID = "ojvtpuvg";

var passwordBuilder = new StringBuilder();

using (var md5 = System.Security.Cryptography.MD5.Create())
{
    for (long i = 0; passwordBuilder.Length < 8; i++)
    {
        var input = Encoding.ASCII.GetBytes(doorID + i.ToString());
        var hashbytes = md5.ComputeHash(input);

        var hexRepresentation = Convert.ToHexString(hashbytes);

        if (hexRepresentation[..5] == "00000")
        {
            passwordBuilder.Append(hexRepresentation[5]);
        }
    }
}

Console.WriteLine($"Part 1: {passwordBuilder}");