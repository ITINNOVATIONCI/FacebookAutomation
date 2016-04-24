<?php
include_once('conn.php');
if(isset($_POST['email_address']))
{
	$email_address = mysql_real_escape_string(trim($_POST['email_address']));
	$sql="SELECT count(*) as total FROM newsletter_signup WHERE `email_address` = '$email_address'";
	$result=mysql_query($sql);
	$row=mysql_fetch_array($result);
	if($row['total'] != 0)
	{
		echo "<div class='alert-danger'><b>Vous êtes d&eacute;j&agrave; abonn&eacute; &agrave; notre newsletter.</b></div>";
	}
	else
	{
		mysql_query("INSERT INTO newsletter_signup SET `email_address` = '$email_address'");
		echo "<div class='alert-success'><b>Votre email a &eacute;t&eacute; enregistr&eacute;.</b></div>";
	}
}
?>  
  