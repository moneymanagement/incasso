//using Abp.BackgroundJobs;
//using Abp.Dependency;
//using incasso.Jobs.UploadJob;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace incasso.Jobs
//{
//    public class JobService:ITransientDependency
//    {

//        public JobService(IBackgroundJobManager backgroundJobManager)
//        {
//            _backgroundJobManager = backgroundJobManager;
//        }

//        public void ProcessUploadFile(int uploadId)
//        {
//           _backgroundJobManager.Enqueue<ProcessUploadFile, int>(uploadId,delay: new TimeSpan(0,0,30));
//        }
//    }
//} 
