var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
	res.sendfile('views/chat.html');
  //res.render('chat', { 'title': 'Runtor.Chat','layout':false });
});


module.exports = router;
