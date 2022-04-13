using System.Text;

//string doorID = "abc"; // debug string
string doorID = "ojvtpuvg";

var naivePasswordBuilder = new StringBuilder();
var complexPasswordArray = new char[8];

using (var md5 = System.Security.Cryptography.MD5.Create())
{
    for (long i = 0, charsMissing = 8; charsMissing > 0; i++)
    {
        var input = Encoding.ASCII.GetBytes(doorID + i.ToString());
        var hashbytes = md5.ComputeHash(input);

        var hexRepresentation = Convert.ToHexString(hashbytes);

        if (hexRepresentation[..5] == "00000")
        {
            naivePasswordBuilder.Append(hexRepresentation[5]);

            var position = hexRepresentation[5] - '0';
            
            if (position >= 0 && position < 8 && complexPasswordArray[position] == 0)
            {
                complexPasswordArray[position] = hexRepresentation[6];
                charsMissing--;
            }
        }
    }
}

Console.WriteLine($"Part 1: {naivePasswordBuilder.ToString().Substring(0, 8)}");
Console.WriteLine($"Part 2: {new string(complexPasswordArray)}");