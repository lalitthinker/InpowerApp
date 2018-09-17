using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AppCenter.Crashes;

namespace InPowerIOS.Common
{
        public class AWSUploader
        {
            public static async Task AWSUploadImage(string filePath, string requiredFileName)
            {
                try
                {

                AWSCredentials creds = new BasicAWSCredentials(CommonHelper.GetAWSAccessKey(),
                                                                   CommonHelper.GetAWSSecretKey());

                    var client = new AmazonS3Client(creds, RegionEndpoint.USWest2);

                    var po = new PutObjectRequest();
                    po.CannedACL = S3CannedACL.PublicReadWrite;
                    po.FilePath = filePath;
                    po.BucketName = "inpower-images";
                    po.Key = requiredFileName;

                    await client.PutObjectAsync(po);


                    Console.WriteLine("Upload completed");
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        throw new Exception("Check the provided AWS Credentials.");
                    }
                    else
                    {
                        throw new Exception("Error occurred: " + amazonS3Exception.Message);
                    }
                }
            }

            public static async Task AWSUploadAudioVideo(string filePath, string requiredFileName, string mediatype)
            {
                try
                {
                    //   CommonConstants.
          
                    AWSCredentials creds = new BasicAWSCredentials(CommonHelper.GetAWSAccessKey(),
                                                                   CommonHelper.GetAWSSecretKey());
                    //    TransferUtilityConfig config = new TransferUtilityConfig () { ConcurrentServiceRequests = 5 };
                    var client = new AmazonS3Client(creds, RegionEndpoint.USWest2);
                    //    TransferUtility trans = new TransferUtility (client, config);
                    var po = new PutObjectRequest();
                    po.CannedACL = S3CannedACL.PublicReadWrite;
                    po.FilePath = filePath;
                    if (mediatype == "Audio")
                    {
                    po.BucketName = "inpower-audio";

                    }
                    if (mediatype == "Video")
                    {
                        po.BucketName = "inpower-video";

                    }
                    po.Key = requiredFileName;
                    await client.PutObjectAsync(po);

                    //   var go = new GetObjectRequest ();
                    //   go.BucketName = "bitmob-videos";
                    //   go.Key = "abcdefhij.jpg";
                    //   var res = await client.GetObjectAsync (go);

                    Console.WriteLine("Upload completed");
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                        ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        throw new Exception("Check the provided AWS Credentials.");
                    }
                    else
                    {
                        throw new Exception("Error occurred: " + amazonS3Exception.Message);
                    }
                 
                }
            }





            public static string SetMediaName(string mediaType)
            {
                var name = "";
                if (mediaType == "Photo")
                {
                    name = "inpower-" + Guid.NewGuid() + ".jpg";
                    return name;
                }
                if (mediaType == "Video")
                {
                name = "inpower-" + Guid.NewGuid() + ".mp4";
                    return name;
                }
                if (mediaType == "Audio")
                {
                name = "inpower-" + Guid.NewGuid() + ".mp3";
                    return name;
                }
                throw new Exception("Unknown media type upload.");
            }

            public static string GetMediaUrl(string mediaType)
            {
                //   string name = "";
                if (mediaType == "Photo")
                {
                    return "http://d25g1coazea76c.cloudfront.net/";
                }
                if (mediaType == "Video")
                {
                    return "http://d25g1coazea76c.cloudfront.net/";
                }
                if (mediaType == "Audio")
                {
                    return "http://d2qthobkt3v2pa.cloudfront.net/";
                }
                throw new Exception("Unknown media type upload.");
            }

        }
    }
