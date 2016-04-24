<?php
include_once('conn.php');
if(isset($_POST['submit']) && $_POST['submit'] != '')
{
	$sender_email = trim($_POST['sender_email']);
	$subject = trim($_POST['subject']);
	$message = trim($_POST['message']);
	$random_hash = md5(date('r', time())); 
	$myFile = "$random_hash.txt";
	$fh = fopen($myFile, 'w') or die("can't open file");
	$headers = "From: $sender_email\r\nReply-To: $sender_email";
	$headers .= "\r\nContent-Type: multipart/alternative; boundary=\"PHP-alt-".$random_hash."\""; 
	$sql="SELECT email_address FROM newsletter_signup";
	$result = mysql_query($sql);
	while($row=mysql_fetch_array($result))
	{
		$to = trim($row['email_address']);
		$mail_sent = @mail($to, $subject, $message, $headers);
		$stringData = $to.' Send mail successfully'.PHP_EOL;
		fwrite($fh, $stringData);
	}
	fclose($fh);
	
	
	$messagee = "Mail Send Report";
	$file_size = filesize($myFile);
    $handle = fopen($myFile, "r");
    $content = fread($handle, $file_size);
    fclose($handle);
    $content = chunk_split(base64_encode($content));
    $uid = md5(uniqid(time()));
    $name = basename($myFile);
    $header = "From: ".$sender_email." <".$sender_email.">\r\n";
    $header .= "Reply-To: ".$sender_email."\r\n";
    $header .= "MIME-Version: 1.0\r\n";
    $header .= "Content-Type: multipart/mixed; boundary=\"".$uid."\"\r\n\r\n";
    $header .= "This is a multi-part message in MIME format.\r\n";
    $header .= "--".$uid."\r\n";
    $header .= "Content-type:text/plain; charset=iso-8859-1\r\n";
    $header .= "Content-Transfer-Encoding: 7bit\r\n\r\n";
    $header .= $messagee."\r\n\r\n";
    $header .= "--".$uid."\r\n";
    $header .= "Content-Type: application/octet-stream; name=\"".$myFile."\"\r\n"; // use different content types here
    $header .= "Content-Transfer-Encoding: base64\r\n";
    $header .= "Content-Disposition: attachment; filename=\"".$myFile."\"\r\n\r\n";
    $header .= $content."\r\n\r\n";
	$header .= "--".$uid."--";
    if (mail($sender_email, $subject, "", $header)) {
        echo "mail send ... OK"; // or use booleans here
    } else {
        echo "mail send ... ERROR!";
    }
}
?>
<div class="serch-wrap">
	<form action="" method="post">
	<p>Sender Email Address:<input type="mail" name="sender_email" /></p>
	<p>Subject:<input type="text" name="subject" /></p>
	<p>Message:<textarea name="message" rows="6" cols="50"></textarea></p>
	<input type="submit" name="submit" value="Send Mail" />
	</form>
</div>	