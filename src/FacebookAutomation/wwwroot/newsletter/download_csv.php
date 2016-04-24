<?php
if(isset($_POST['download']) && $_POST['download'] != '')
{
	/* csv file generate */
	$filename = tempnam(sys_get_temp_dir(), "csv");
	$file = fopen($filename,"w");
	$StoreField = array('Email No','Email Address');
	fputcsv($file,$StoreField);
	
	include_once('conn.php');
	$sql="SELECT * FROM newsletter_signup";
	$result = mysqli_query($bdd,$sql);
	while($row=mysqli_fetch_assoc($result))
	{
		$StoreFieldValue = array($row['email_id'],$row['email_address']);
		fputcsv($file,$StoreFieldValue);
	}
	mysqli_free_result($result);
	fclose($file);
	/* send file to browser */
	header("Content-Type: application/csv");
	header("Content-Disposition: attachment;Filename=allemail".time().".csv");
	readfile($filename);
	unlink($filename);
	
}
?>