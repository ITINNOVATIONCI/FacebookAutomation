<?php
//mysql_connect('localhost','root','laressource');
//mysql_select_db('sipma_mail');
if($bdd = mysqli_connect('localhost', 'root', 'laressource', 'sipma_mail'))

{

    // Si la connexion a réussi, rien ne se passe.

}

else // Mais si elle rate…

{

    echo 'Erreur'; // On affiche un message d'erreur.

}
?>