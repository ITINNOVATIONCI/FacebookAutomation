<!DOCTYPE HTML>
<html lang="en-US">
<head>
	<meta charset="UTF-8">
	<title>Newsletter Signup All Email Address</title>
	<link rel="stylesheet" href="css/style.css" />
	<script>
	function csv(){
	document.print_csv.submit();
	}
	</script>
</head>
<body>
	<?php
	include_once('conn.php');
	$sql="SELECT * FROM newsletter_signup";
	$result = mysqli_query($bdd,$sql);
	if (!$result) die('mySQL error: ' . mysql_error());
	echo "<style>
	table { border:1px solid #ccc; border-collapse:collapse; padding:5px; margin-bottom:5px;width:400px}
	th { background:#eff5fc; border:1px solid #ccc; padding:10px; text-align:center; }
	td { padding:10px; border:1px solid #ccc;}
	</style>";
	?>
	<div class="serch-wrap">
	<form name="print_csv" action="download_csv.php" method="post">
	<input type="hidden" value="csv" name="download" />
	<?php
	echo "<table>";
	echo "<th>ID</th><th>Email Address</th>";	
	if (mysqli_num_rows($result) == 0) 
	{
		echo '<tr>';
		echo "<td colspan='2'>Aucun email trouvé.</td>";
		echo '</tr>';
	}
	else
	{
		$emails=array();
		while($row=mysqli_fetch_assoc($result))
		{
			$emails[] = $row;
		}
		mysqli_free_result($result);
	}
	foreach($emails as $email)
	{
		echo '<tr>';
		echo '<td>'.$email['email_id'].'</td><td>'.$email['email_address'].'</td>';
		echo '</tr>';
	}
	echo "</table>";
	?>
	</form>
	<input name="downloadCSV" type="button" value="Telecharger le fichier CSV" class="button-link" onClick="csv()" />
	<a class="button-link" href="send_email.php" >Envoyer un email</a>
	</div>
		
</body>
</html>