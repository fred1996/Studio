using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Online.Web.DAL;
using Online.DbHelper.Model;
using System.Data.Entity;
using System.Reflection;
using Online.DbHelper.BLL;
using Online.DbHelper.Common;

namespace Online.Web.Controllers
{
    public class VoteController : BaseController
    {
        // GET: Vote
        public ActionResult Index()
        {
            try
            {
                List<UserVote> model = GetUserVotes();
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex,GetType(),MethodBase.GetCurrentMethod().Name);
                return View();
            }
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public List<UserVote> GetUserVotes()
        {
            try
            {
                List<UserVote> vote = new List<UserVote>();
                if (RoomId != 0)
                {
                    vote = DataSource.UserVotes.Where(t => t.RoomID == RoomId && t.IsDeleted == false).ToList();
                }
                return vote;
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }

        }
        public ActionResult GetUserVoteColum(int? vid)
        {
            try
            {
                DataSource.Configuration.ProxyCreationEnabled = false;
                var list = DataSource.UserVoteColums.Where(x => x.VoteID == vid);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult DelUserVoteColum(int id)
        {
            try
            {
                var vote = DataSource.UserVoteColums.FirstOrDefault(x => x.ID == id);
                if (vote != null)
                {
                    DataSource.UserVoteColums.Remove(vote);
                    DataSource.SaveChanges();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUserVoteColum(int vid, string name)
        {
            try
            {
                UserVoteColum colum = new UserVoteColum() { VoteID = vid, Columname = name, VoteCount = 0 };
                DataSource.UserVoteColums.Add(colum);
                DataSource.SaveChanges();
                return Json(colum.ID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult delVote(int? vid)
        {
            try
            {
                int i = 0;
                UserVote vote = DataSource.UserVotes.FirstOrDefault(t => t.VoteID == vid);
                if (vote != null)
                {
                    DataSource.UserVotes.Remove(vote);
                    i = DataSource.SaveChanges();
                }
                var result = new { start = i };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult addVote(string usercount,string votetitle,string option,
                                    DateTime betime,DateTime endtime,string anony,string mult,
                                    string viewresult,string detail)
        {
            try
            {
                int i = 0;
                UserVote vote = new UserVote();
                vote.RoomID = RoomId;
                vote.CreateUser = Users != null ? Users.UserName : "";
                vote.IsDeleted = false;
                vote.OptCount = string.IsNullOrEmpty(usercount)?1:  Convert.ToInt32( usercount )  ;
                vote.VoteCount = 0;
                vote.VoteTitle = votetitle ;
                string op =  option;
                string[] opc = op.Split(',');
                vote.VoteBeginTime = betime ;
                vote.VoteEndTime =  endtime ;
                vote.IsAnonymous =  anony  == "1";
                vote.IsVoteMulti = mult  == "1";
                vote.IsViewResult =  viewresult  == "1";
                vote.VoteSummary =  detail ;
                vote.CreateTime = DateTime.Now;
                using (var db = new DataContextBll())
                {
                    if (!db.UserVotes.Any(t => t.VoteTitle == vote.VoteTitle))
                    {
                        db.UserVotes.Add(vote);
                        i = db.SaveChanges();
                        for (var j = 0; j < opc.Length; j++)
                        {
                            UserVoteColum colum = new UserVoteColum()
                            {
                                Columname = opc[j],
                                VoteCount = 0,
                                VoteID = vote.VoteID
                            };
                            db.UserVoteColums.Add(colum);
                            db.SaveChanges();
                        }

                    }
                    else
                    {
                        i = -1;
                    }
                    var result = new { start = i };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }
        public ActionResult editVote(int id,string  usercount,string votetitle,string  endtime,
             string anony,string mult,string viewresult,string detail)
        {
            try
            {
                using (var db = new DataContextBll())
                {
                    int i = -1;
                    UserVote voteModel = db.UserVotes.FirstOrDefault(x => x.VoteID == id);
                    if (voteModel != null)
                    {
                        voteModel.OptCount =string.IsNullOrEmpty(usercount)?1:Convert.ToInt32(usercount);
                        voteModel.VoteTitle = votetitle ;
                        voteModel.VoteEndTime =string.IsNullOrEmpty(endtime)? DateTime.MinValue:Convert.ToDateTime(endtime);
                        voteModel.IsAnonymous =  anony  == "1"  ;
                        voteModel.IsVoteMulti =  mult  == "1";
                        voteModel.IsViewResult = viewresult  == "1";
                        voteModel.VoteSummary =  detail ;
                        i = db.SaveChanges();
                    }
                    var result = new { start = i };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public ActionResult GetUserVoteResult()
        {
            try
            {
                var userVotes = DataSource.UserVotes.Where(x => x.RoomID == RoomId && x.IsDeleted != true && x.IsViewResult).ToList();
                var results = userVotes.Select(x => new
                {
                    x.VoteTitle,
                    x.OptCount,
                    x.VoteCount,
                    x.VoteID,
                    x.CreateUser,
                    UserVoteColums = x.UserVoteColums.Select(t => new
                    {
                        t.ID,
                        t.Columname,
                        t.VoteCount
                    })
                });
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.WriteError(ex, GetType(), MethodBase.GetCurrentMethod().Name);
                throw;
            }
        }

        public int GetUserVoteResultCount()
        {
            return 0;
        }
    }
}