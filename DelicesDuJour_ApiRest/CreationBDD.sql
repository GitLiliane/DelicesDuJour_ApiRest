SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;ALTER SCHEMA public OWNER TO postgres;COMMENT ON SCHEMA public IS '';
SET default_tablespace = '';SET default_table_access_method = heap;
CREATE TABLE public.avis (
    id_recette integer NOT NULL,
    id_utilisateur integer NOT NULL,
    note integer NOT NULL,
    commentaire character varying(500),
    date_commentaire date,
    CONSTRAINT avis_note_check CHECK (((note >= 1) AND (note <= 5)))
);
ALTER TABLE public.avis OWNER TO postgres;
CREATE TABLE public.categories (
    id integer NOT NULL,
    nom character varying(50) NOT NULL
);
ALTER TABLE public.categories OWNER TO postgres;
CREATE SEQUENCE public.categories_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE public.categories_id_seq OWNER TO postgres;ALTER SEQUENCE public.categories_id_seq OWNED BY public.categories.id;CREATE TABLE public.categories_recettes (
    id_categorie integer NOT NULL,
    id_recette integer NOT NULL
);
ALTER TABLE public.categories_recettes OWNER TO postgres;
CREATE TABLE public.etapes (
    id_recette integer NOT NULL,
    numero integer NOT NULL,
    titre character varying(200),
    texte character varying(5000)
);
ALTER TABLE public.etapes OWNER TO postgres;
CREATE TABLE public.ingredients (
    id integer NOT NULL,
    nom character varying(50) NOT NULL
);
ALTER TABLE public.ingredients OWNER TO postgres;
CREATE SEQUENCE public.ingredients_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE public.ingredients_id_seq OWNER TO postgres;ALTER SEQUENCE public.ingredients_id_seq OWNED BY public.ingredients.id;CREATE TABLE public.ingredients_recettes (
    id_ingredient integer NOT NULL,
    id_recette integer NOT NULL,
    quantite character varying(40)
);
ALTER TABLE public.ingredients_recettes OWNER TO postgres;
CREATE TABLE public.recettes (
    id integer NOT NULL,
    nom character varying(100) NOT NULL,
    temps_preparation interval NOT NULL,
    temps_cuisson interval NOT NULL,
    difficulte integer NOT NULL,
    photo character varying(100) DEFAULT 'default.jpg'::character varying,
    id_utilisateur integer,
    CONSTRAINT recettes_difficulte_check CHECK (((difficulte >= 1) AND (difficulte <= 3)))
);
ALTER TABLE public.recettes OWNER TO postgres;
CREATE SEQUENCE public.recettes_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE public.recettes_id_seq OWNER TO postgres;ALTER SEQUENCE public.recettes_id_seq OWNED BY public.recettes.id;CREATE TABLE public.roles (
    id integer NOT NULL,
    nom character varying(100) NOT NULL
);
ALTER TABLE public.roles OWNER TO postgres;
CREATE SEQUENCE public.roles_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE public.roles_id_seq OWNER TO postgres;ALTER SEQUENCE public.roles_id_seq OWNED BY public.roles.id;CREATE TABLE public.utilisateurs (
    id integer NOT NULL,
    identifiant character varying(20) NOT NULL,
    email character varying(50) NOT NULL,
    pass_word character varying(250) NOT NULL,
    role_id integer
);
ALTER TABLE public.utilisateurs OWNER TO postgres;
CREATE SEQUENCE public.utilisateurs_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
ALTER SEQUENCE public.utilisateurs_id_seq OWNER TO postgres;ALTER SEQUENCE public.utilisateurs_id_seq OWNED BY public.utilisateurs.id;ALTER TABLE ONLY public.categories ALTER COLUMN id SET DEFAULT nextval('public.categories_id_seq'::regclass);ALTER TABLE ONLY public.ingredients ALTER COLUMN id SET DEFAULT nextval('public.ingredients_id_seq'::regclass);ALTER TABLE ONLY public.recettes ALTER COLUMN id SET DEFAULT nextval('public.recettes_id_seq'::regclass);ALTER TABLE ONLY public.roles ALTER COLUMN id SET DEFAULT nextval('public.roles_id_seq'::regclass);ALTER TABLE ONLY public.utilisateurs ALTER COLUMN id SET DEFAULT nextval('public.utilisateurs_id_seq'::regclass);
COPY public.avis (id_recette, id_utilisateur, note, commentaire, date_commentaire) FROM stdin;
1	1	4	Super bon !	2024-05-21
28	3	5	Quelle belle recette, cela donne envie !	2025-04-10
\.
COPY public.categories (id, nom) FROM stdin;
1	Entrée
2	Plat
3	Dessert
4	Soupe
\.
COPY public.categories_recettes (id_categorie, id_recette) FROM stdin;
2	24
2	39
2	25
1	26
2	26
1	28
2	28
1	38
2	38
1	1
1	22
2	22
4	22
1	32
2	32
1	29
2	29
1	36
2	36
1	30
2	30
2	31
1	37
1	34
2	34
1	61
4	61
1	66
4	66
3	50
3	75
3	76
1	65
4	65
1	62
4	62
1	83
4	83
2	85
4	85
2	78
4	78
1	79
2	79
4	79
3	27
3	43
3	45
3	44
2	18
2	53
3	46
3	42
3	48
3	41
3	47
1	19
2	19
3	51
3	49
2	20
2	35
3	52
2	40
2	55
2	21
1	23
2	23
2	33
\.
COPY public.etapes (id_recette, numero, titre, texte) FROM stdin;
45	1	Préparer la base	Réduis les biscuits en miettes, ajoute le beurre fondu et mélange bien. Tasse ce mélange au fond d’un moule à charnière recouvert de papier cuisson. Mets au réfrigérateur pendant que tu prépares la garniture.
1	1	étape 1	Faire tremper les champignons noirs dans de l’eau chaude pendant 15 min, puis les émincer finement.
1	2	étape 2	Faire cuire les vermicelles de riz dans l’eau bouillante pendant 3 min, égoutter, puis couper grossièrement.
1	3	étape 3	Mélanger dans un grand bol : la viande hachée, les champignons, les vermicelles, la carotte râpée, l’oignon haché, l’œuf, la sauce nuoc-mâm, le sucre et le poivre.
58	1	Préparer la crème anglaise à la vanille	Dans une casserole, fais chauffer la crème liquide avec le sucre vanillé jusqu’à ce qu’elle soit chaude mais sans bouillir. Pendant ce temps, fouette les jaunes d’œufs avec le sucre dans un bol. Verse progressivement la crème chaude sur les œufs en mélangeant sans arrêt, puis remets le tout sur feu doux. Laisse épaissir doucement en remuant, jusqu’à ce que la crème nappe la cuillère. Retire du feu et laisse complètement refroidir.
58	2	Préparer le coulis de framboises	Dans un mixeur, mélange les framboises avec le sucre glace et le jus de citron jusqu’à obtenir une purée homogène. Passe le mélange au tamis pour retirer les graines si tu veux une texture bien lisse. Réserve au réfrigérateur le temps que la crème vanille refroidisse.
58	3	Assembler la bûche	Chemise un moule à cake avec du film plastique pour faciliter le démoulage. Verse une couche de crème vanille, puis ajoute une couche de coulis de framboises. Répète l’opération jusqu’à épuisement des préparations, en terminant par une fine couche de biscuits sablés écrasés pour former le fond de la bûche.
58	4	Congélation	Place le moule au congélateur pendant au moins 6 heures, ou idéalement toute une nuit, afin que la bûche prenne bien et que les couches se solidifient.
43	1	Préparer le caramel	Dans une casserole à fond épais, fais chauffer doucement le sucre sans remuer jusqu’à ce qu’il fonde et prenne une belle couleur dorée. Retire la casserole du feu, ajoute le beurre en morceaux, puis la crème liquide en mélangeant bien pour obtenir un caramel lisse. Laisse tiédir à température ambiante.
43	2	Faire fondre le chocolat	Fais fondre le chocolat noir et le chocolat au lait ensemble au bain-marie ou au micro-ondes par petites étapes. Remue jusqu’à obtenir une texture bien lisse et homogène.
43	3	Assembler les carrés	Dans un moule carré recouvert de papier sulfurisé, verse une première couche de chocolat fondu, puis étale le caramel sur toute la surface. Recouvre avec le reste du chocolat et parsème de biscuits sablés émiettés. Lisse la surface avec une spatule.
43	4	Refroidissement	Place le moule au réfrigérateur pendant environ 2 heures, jusqu’à ce que le chocolat soit bien pris. Une fois durci, découpe le tout en petits carrés réguliers à l’aide d’un couteau chaud.
43	5	Service	Dispose les carrés sur une assiette, conserve-les au frais jusqu’au moment de servir. Ils se dégustent aussi bien en dessert qu’en petit encas gourmand avec le café.
45	2	Préparer la crème au citron	Dans un saladier, bats le fromage frais avec le sucre jusqu’à ce que la texture soit lisse. Ajoute les œufs un à un, puis la farine, le jus et le zeste de citron. Mélange jusqu’à obtenir une crème homogène.
45	3	Cuisson du cheesecake	Verse la préparation sur la base biscuitée, lisse la surface et enfourne à 160 °C pendant environ 40 minutes, jusqu’à ce que le centre soit juste pris. Laisse refroidir complètement dans le four porte entrouverte.
44	1	Faire fondre le chocolat	Fais fondre le chocolat noir avec la crème liquide à feu doux ou au bain-marie jusqu’à obtenir une texture lisse.
44	2	Incorporer la noisette	Ajoute la pâte de noisette et mélange bien pour obtenir une préparation homogène. Verse dans de petits moules ou un plat.
44	3	Finaliser et refroidir	Parseme de noisettes concassées, laisse refroidir à température ambiante puis place au réfrigérateur pendant 1 heure avant dégustation.
18	1	Préparer la choucroute	Rincez la choucroute à l’eau froide et égouttez-la. Faites-la revenir quelques minutes avec l’oignon émincé et le laurier pour en relever le goût.
18	2	Cuire les viandes	Dans une grande casserole, déposez le lard fumé et les saucisses. Couvrez d’eau et laissez mijoter doucement pendant 1 heure.
18	3	Ajouter les pommes de terre et assaisonner	Ajoutez les pommes de terre pelées et coupées en morceaux. Salez et poivrez selon votre goût. Poursuivez la cuisson 30 minutes jusqu’à ce que les pommes de terre soient tendres.
53	1	Préparer les légumes	Épluchez et coupez les carottes et courgettes en morceaux. Émincez l’oignon.
53	2	Cuire la viande et les légumes	Dans une grande casserole, faites revenir la viande avec l’oignon dans l’huile d’olive. Ajoutez les carottes, couvrez d’eau et laissez mijoter 40 minutes. Ajoutez ensuite les courgettes et les pois chiches, puis poursuivez la cuisson 20 minutes. Assaisonnez avec sel, poivre et ras-el-hanout.
53	3	Cuire la semoule	Versez la semoule dans un grand bol, ajoutez un peu d’eau et laissez gonfler 5 minutes. Égrenez avec une fourchette pour séparer les grains.
53	4	Servir le couscous	Disposez la semoule dans un plat, ajoutez la viande et les légumes par-dessus. Arrosez du bouillon de cuisson selon votre goût. Servez chaud.
46	1	Préparer le mélange	Dans une casserole, mélangez le sucre, le cacao et la maïzena. Ajoutez progressivement le lait en remuant pour éviter les grumeaux.
46	2	Cuire la crème	Faites chauffer à feu doux tout en remuant jusqu’à ce que le mélange épaississe. Ajoutez l’extrait de vanille et mélangez bien.
46	3	Refroidir et servir	Versez la crème dans des ramequins. Laissez refroidir à température ambiante puis placez au réfrigérateur 1 à 2 heures avant de déguster.
42	1	Mixer les bananes	Épluchez les bananes et réduisez-les en purée.
42	2	Préparer la crème	Dans une casserole, mélangez le sucre et la maïzena. Ajoutez progressivement le lait en remuant pour éviter les grumeaux.
42	3	Cuire avec les bananes	Ajoutez la purée de bananes au mélange et faites chauffer à feu doux tout en remuant jusqu’à ce que la crème épaississe. Incorporez l’extrait de vanille.
42	4	Refroidir et servir	Versez la crème dans des ramequins. Laissez refroidir à température ambiante puis placez au réfrigérateur 1 à 2 heures avant de déguster.
48	1	Préparer la base	Émiettez les biscuits dans un bol. Faites fondre le beurre et mélangez-le avec le sucre et le cacao, puis incorporez aux biscuits.
48	2	Ajouter les noix	Si vous le souhaitez, ajoutez les noix ou amandes hachées et mélangez bien le tout.
48	3	Former le dessert	Versez le mélange dans un moule ou un plat recouvert de papier sulfurisé et tassez légèrement.
48	4	Refroidir et servir	Placez le dessert au réfrigérateur pendant 1 à 2 heures jusqu’à ce qu’il soit ferme, puis coupez-le en carrés pour servir.
41	1	Préparer les fruits	Lavez et équeutez les fruits rouges, puis placez-les dans un bol. Saupoudrez-les de sucre et mélangez délicatement.
41	2	Monter la crème	Fouettez la crème liquide avec l’extrait de vanille jusqu’à obtenir une consistance ferme.
41	3	Assembler le dessert	Dans des verrines, déposez une couche de fruits, puis une couche de crème fouettée. Répétez si nécessaire.
41	4	Réfrigérer et servir	Placez le dessert au réfrigérateur pendant au moins 30 minutes avant de servir pour qu’il soit frais.
47	1	Préparer les fruits	Lavez et découpez les fruits en petits morceaux, puis placez-les dans un bol.
47	2	Mélanger le yaourt	Versez le yaourt dans un autre bol et ajoutez-y le miel et les graines de chia si vous le souhaitez, puis mélangez bien.
47	3	Assembler le dessert	Dans des verrines ou des bols, alternez une couche de fruits et une couche de yaourt jusqu’à remplir les contenants.
47	4	Réfrigérer et servir	Placez le dessert au réfrigérateur pendant 10 à 15 minutes avant de servir afin qu’il soit bien frais.
19	1	Préparer la salade	Lavez la salade et la tomate, puis coupez la tomate en dés et disposez le tout dans un plat.
19	2	Cuire le poulet	Assaisonnez les filets de poulet avec du sel et du poivre. Faites-les cuire à la poêle avec l’huile d’olive jusqu’à ce qu’ils soient dorés et bien cuits.
19	3	Assembler le plat	Déposez les filets de poulet sur le lit de salade et tomates.
19	4	Servir	Servez immédiatement, éventuellement accompagné d’un filet de vinaigre balsamique ou de jus de citron.
51	1	Préparer les ingrédients	Préchauffez votre four à 180°C. Faites fondre le chocolat et le beurre ensemble dans une casserole ou au micro-ondes jusqu’à obtention d’un mélange homogène.
51	2	Mélanger les ingrédients	Dans un bol, battez les œufs avec le sucre. Incorporez ensuite le mélange chocolat-beurre fondu, puis ajoutez la farine en remuant doucement.
51	3	Cuire le gâteau	Versez la pâte dans un moule beurré et fariné. Enfournez pendant environ 25 minutes. Vérifiez la cuisson avec la pointe d’un couteau : elle doit ressortir propre.
51	4	Servir	Laissez refroidir quelques minutes avant de démouler et servez tel quel ou avec un peu de crème ou sucre glace selon votre goût.
49	1	Préparer les ingrédients	Préchauffez votre four à 180°C. Beurrez et farinez un moule.
49	2	Mélanger la pâte	Dans un bol, battez les œufs avec le sucre jusqu’à ce que le mélange blanchisse. Incorporez ensuite le beurre fondu et la farine, en mélangeant doucement.
49	3	Ajouter les fruits	Incorporez délicatement les fruits rouges à la pâte afin de ne pas les écraser.
49	4	Cuire le gâteau	Versez la pâte dans le moule et enfournez pendant environ 25 minutes. Vérifiez la cuisson avec la pointe d’un couteau : elle doit ressortir propre.
49	5	Servir	Laissez refroidir quelques minutes avant de démouler. Servez nature ou avec un peu de crème chantilly selon votre goût.
20	1	Préparer les ingrédients	Coupez la tomate en rondelles et lavez les feuilles de salade.
20	2	Cuire les steaks	Faites cuire les steaks à votre convenance dans une poêle chaude.
20	3	Assembler le hamburger	Ouvrez les pains, déposez une tranche de fromage sur chaque steak chaud, puis ajoutez la salade et la tomate.
20	4	Ajouter les sauces	Nappez les steaks de ketchup et/ou mayonnaise selon votre goût.
20	5	Servir	Refermez les pains et servez immédiatement, accompagnés éventuellement de frites.
35	1	Préparer la garniture	Émincez l’oignon et faites-le revenir avec la viande hachée jusqu’à ce qu’elle soit dorée. Ajoutez la sauce tomate, salez et poivrez.
35	2	Monter les lasagnes	Dans un plat, déposez une couche de feuilles de lasagnes, puis la garniture, et parsemez de fromage râpé. Répétez jusqu’à épuisement des ingrédients.
35	3	Cuire les lasagnes	Enfournez le plat à 180°C pendant 35 à 40 minutes jusqu’à ce que le dessus soit doré.
35	4	Servir	Laissez reposer quelques minutes avant de découper et servir.
52	1	Préparer les œufs	Séparez les blancs des jaunes. Battez les jaunes avec le sucre jusqu’à obtenir un mélange crémeux.
52	2	Incorporer le mascarpone	Ajoutez le mascarpone aux jaunes sucrés et mélangez jusqu’à obtenir une texture lisse.
52	3	Monter les blancs en neige	Battez les blancs en neige ferme et incorporez-les délicatement au mélange mascarpone-jaunes.
52	4	Servir	Répartissez dans des verrines et laissez reposer au frais avant de déguster.
40	1	Préparer les ingrédients	Lavez le crabe et coupez les légumes en petits morceaux.
40	2	Faire revenir	Dans une casserole, faites chauffer l’huile, puis faites revenir l’oignon, l’ail et la tomate jusqu’à ce qu’ils soient tendres.
40	3	Cuire le crabe	Ajoutez le crabe et laissez cuire quelques minutes jusqu’à ce qu’il devienne rouge.
40	4	Ajouter le riz et l’eau	Versez le riz et assez d’eau pour cuire le tout. Salez et poivrez. Couvrez et laissez mijoter jusqu’à ce que le riz soit tendre.
40	5	Servir	Mélangez délicatement et servez chaud.
55	1	Préparer les ingrédients	Lavez les moules et épluchez les pommes de terre. Coupez-les en bâtonnets pour les frites.
55	2	Cuire les frites	Faites chauffer de l’huile dans une friteuse ou une poêle et faites frire les pommes de terre jusqu’à ce qu’elles soient dorées. Égouttez-les sur du papier absorbant et salez.
55	3	Cuire les moules	Dans une grande casserole, faites fondre le beurre, ajoutez l’oignon et l’ail hachés, puis incorporez les moules. Couvrez et laissez cuire 5 à 7 minutes jusqu’à ce que les moules s’ouvrent. Salez et poivrez.
55	4	Servir	Servez les moules chaudes accompagnées des frites.
54	1	Préparer les ingrédients	Coupez le poulet en morceaux, hachez l’oignon et le poivron, et concassez les tomates.
54	2	Cuire le poulet et les légumes	Dans une grande poêle, faites chauffer l’huile d’olive et faites revenir le poulet jusqu’à ce qu’il soit doré. Ajoutez l’oignon, le poivron et les tomates, puis laissez cuire 5 minutes.
54	3	Ajouter le riz et le bouillon	Incorporez le riz et mélangez bien. Ajoutez le bouillon, salez et poivrez. Couvrez et laissez cuire 15 minutes à feu moyen.
54	4	Ajouter les fruits de mer	Ajoutez les crevettes et les petits pois, puis laissez cuire encore 5 minutes jusqu’à ce que le riz soit tendre et les crevettes cuites.
54	5	Servir	Servez chaud directement dans la poêle pour conserver le charme de la paëlla.
21	1	Préparer les ingrédients	Lavez et coupez les tomates, hachez l’ail et ciselez le basilic.
21	2	Cuire les pâtes	Faites cuire les pâtes dans une grande casserole d’eau bouillante salée selon le temps indiqué sur le paquet.
21	3	Préparer la sauce	Dans une poêle, faites chauffer l’huile d’olive, ajoutez l’ail puis les tomates et laissez cuire 5 minutes. Salez et poivrez.
21	4	Mélanger les pâtes et la sauce	Égouttez les pâtes, ajoutez-les à la poêle avec la sauce, mélangez bien et parsemez de basilic.
21	5	Servir	Servez immédiatement, chaud, avec un filet d’huile d’olive si vous le souhaitez.
23	1	Préparer les ingrédients	Coupez la mozzarella et les tomates en tranches, et ciselez le basilic.
23	2	Préparer la pâte	Étalez la pâte à pizza sur une plaque de cuisson légèrement huilée.
23	3	Garnir la pizza	Disposez les tranches de tomates et de mozzarella sur la pâte, salez, poivrez et arrosez d’un filet d’huile d’olive.
23	4	Cuire la pizza	Enfournez dans un four préchauffé à 220 °C pendant 12 à 15 minutes, jusqu’à ce que la pâte soit dorée et le fromage fondu.
23	5	Ajouter le basilic et servir	Parsemez de basilic frais et servez immédiatement.
33	1	Préparer le poulet	Préchauffez le four à 180 °C. Lavez le poulet et séchez-le avec du papier absorbant.
33	2	Préparer la farce	Mélangez le beurre ramolli avec l’ail haché, le jus de citron et les herbes de Provence. Salez et poivrez.
33	3	Farcir le poulet	Étalez le mélange à l’intérieur du poulet et sous la peau pour parfumer la viande.
33	4	Cuire le poulet	Placez le poulet dans un plat et enfournez pendant environ 1 heure, en arrosant régulièrement avec le jus de cuisson.
33	5	Servir	Laissez reposer 5 minutes avant de découper et de servir chaud.
24	1	Préparer le poulet	Coupez les filets de poulet en morceaux. Assaisonnez-les de sel et de poivre.
24	2	Faire revenir les aromates	Dans une poêle, faites chauffer l’huile d’olive et faites revenir l’oignon émincé et l’ail haché jusqu’à ce qu’ils soient translucides.
24	3	Cuire le poulet	Ajoutez le poulet et faites-le dorer légèrement sur toutes les faces.
24	4	Ajouter le riz et le safran	Incorporez le riz et le safran, mélangez pour bien enrober les grains.
24	5	Cuisson finale	Versez le bouillon de volaille, couvrez et laissez mijoter à feu doux pendant 15 à 20 minutes jusqu’à ce que le riz soit tendre et le liquide absorbé.
39	1	Préparer la langouste	Coupez les queues de langouste en deux dans le sens de la longueur. Disposez-les sur une plaque, chair vers le haut.
39	2	Préparer la marinade	Mélangez le beurre fondu, le jus de citron, l’ail, le paprika, le sel et le poivre. Badigeonnez généreusement la chair des langoustes.
39	3	Griller les queues	Faites-les cuire au gril ou au barbecue pendant environ 5 à 6 minutes, jusqu’à ce que la chair soit légèrement dorée et tendre.
39	4	Servir	Servez aussitôt avec un filet de jus de citron et, si vous le souhaitez, un peu de riz ou une salade fraîche.
25	1	Préparer les ingrédients	Épluchez les légumes et coupez les carottes et les pommes de terre en morceaux. Hachez l’oignon et l’ail. Coupez la viande en cubes moyens.
25	2	Faire revenir la viande	Dans une cocotte, faites chauffer l’huile et faites revenir les morceaux de bœuf jusqu’à ce qu’ils soient bien dorés. Saupoudrez de farine et mélangez pour bien enrober la viande.
25	3	Ajouter les légumes et mijoter	Ajoutez l’oignon, l’ail, le concentré de tomate, le bouillon, le thym et le laurier. Salez et poivrez. Laissez mijoter à feu doux pendant environ 1 heure 30.
25	4	Ajouter les pommes de terre	Incorporez les carottes et les pommes de terre, puis poursuivez la cuisson pendant encore 30 minutes, jusqu’à ce que la viande et les légumes soient tendres.
25	5	Servir chaud	Servez le ragoût bien chaud, accompagné de pain ou de riz si vous le souhaitez.
26	1	Préparer les ingrédients	Faites tremper les vermicelles de riz dans de l’eau chaude pendant quelques minutes, puis égouttez-les. Coupez les crevettes en deux dans le sens de la longueur. Râpez la carotte, coupez le concombre en fines lanières et préparez les feuilles de laitue et les herbes.
26	2	Ramollir les galettes de riz	Remplissez un grand bol d’eau tiède. Trempez une galette de riz quelques secondes jusqu’à ce qu’elle devienne souple, puis déposez-la sur un torchon propre.
26	3	Former les rouleaux	Placez un peu de vermicelles, de crevettes, de carotte, de concombre, de laitue, de menthe et de coriandre au centre de la galette. Repliez les bords, puis roulez délicatement pour bien enfermer la garniture.
26	4	Servir	Disposez les rouleaux sur une assiette, couvrez-les d’un linge humide et réservez au frais jusqu’au moment de servir. Dégustez-les avec une sauce pour nems ou une sauce soja.
85	5	Finition	Retire le bouquet garni, rectifie l’assaisonnement et sers bien chaud avec un peu de persil frais haché.
28	1	Préparer la farce	Épluchez et hachez finement l’oignon, l’ail et la carotte. Dans une poêle, faites revenir le tout avec l’huile d’olive pendant quelques minutes. Ajoutez le bœuf haché, le curry, le sel et le poivre, puis laissez cuire jusqu’à ce que la viande soit bien dorée.
28	2	Former les samossas	Découpez les feuilles de brick en deux. Déposez une cuillère de farce sur une extrémité, puis pliez la feuille en triangle en suivant le bord pour bien enfermer la garniture.
28	3	Cuire les samossas	Faites chauffer un fond d’huile dans une poêle et faites dorer les samossas des deux côtés jusqu’à ce qu’ils soient bien croustillants. Égouttez-les sur du papier absorbant.
28	4	Servir	Servez les samossas chauds, accompagnés d’une sauce au yaourt, d’une sauce piquante ou d’une simple salade verte.
38	1	Préparer la base du soufflé	Faites fondre le beurre dans une casserole, ajoutez la farine et mélangez bien. Versez le lait petit à petit sans cesser de remuer jusqu’à obtenir une sauce épaisse. Salez, poivrez et ajoutez une pincée de muscade.
38	2	Incorporer les œufs et le fromage	Séparez les blancs des jaunes. Hors du feu, ajoutez les jaunes à la béchamel tiédie, puis incorporez le fromage râpé. Montez les blancs en neige ferme et ajoutez-les délicatement à la préparation à l’aide d’une spatule.
38	3	Cuire le soufflé	Versez la préparation dans un moule beurré et enfournez à 180°C pendant environ 30 minutes, jusqu’à ce que le soufflé soit bien doré et gonflé. Servez aussitôt sans attendre pour qu’il ne retombe pas.
22	1	Préparer le bouillon parfumé	Faites griller légèrement les oignons et le gingembre à la poêle, puis ajoutez-les dans une grande casserole avec le bouillon de bœuf, la badiane, la cannelle et les clous de girofle. Laissez mijoter environ 1 heure et demie pour que le bouillon soit bien parfumé.
22	2	Cuire les nouilles et préparer la viande	Faites cuire les nouilles de riz selon les instructions du paquet, puis égouttez-les. Coupez le bœuf en fines tranches. Disposez les nouilles et la viande crue dans des bols individuels.
22	3	Assembler et servir la soupe	Versez le bouillon bien chaud sur les nouilles et la viande afin de les cuire légèrement. Ajoutez un filet de sauce soja, un peu de citron vert, des pousses de soja et quelques feuilles de coriandre avant de servir.
32	1	Préparer le riz à sushi	Rincez le riz plusieurs fois jusqu’à ce que l’eau soit claire, puis faites-le cuire dans l’eau. Une fois cuit, mélangez-le délicatement avec le vinaigre de riz, le sucre et le sel. Laissez refroidir à température ambiante.
32	2	Préparer les garnitures	Découpez le saumon en fines tranches et l’avocat en lamelles. Disposez-les à portée de main avec les feuilles de nori et un bol d’eau pour humidifier vos doigts.
32	3	Rouler les sushis	Étalez une fine couche de riz sur une feuille de nori, laissez un bord libre. Disposez au centre quelques tranches de saumon et d’avocat, puis roulez fermement le sushi à l’aide d’un tapis de bambou. Coupez le rouleau en tronçons réguliers.
32	4	Servir	Disposez les sushis dans une assiette. Servez-les avec un peu de sauce soja, du wasabi et du gingembre mariné.
29	1	Préparer la garniture	Faites chauffer l’huile d’olive dans une poêle, puis ajoutez l’oignon émincé et le poivron coupé en dés. Laissez revenir quelques minutes avant d’ajouter le bœuf haché. Assaisonnez avec le paprika, le cumin, le sel et le poivre. Faites cuire jusqu’à ce que la viande soit bien dorée.
29	2	Assembler les tacos	Réchauffez légèrement les tortillas à la poêle ou au micro-ondes. Étalez un peu de sauce au centre, puis ajoutez la garniture de viande, quelques dés de tomate fraîche et du fromage râpé.
29	3	Plier et servir	Repliez les tortillas pour former les tacos. Servez immédiatement, accompagnés d’un peu de sauce blanche ou piquante selon votre goût.
36	1	Préparer la garniture	Dans une poêle, faites revenir l’oignon émincé et l’ail haché dans l’huile d’olive. Ajoutez les crevettes et laissez cuire quelques minutes jusqu’à ce qu’elles soient légèrement dorées. Assaisonnez avec du sel, du poivre et du persil.
36	2	Préparer l’appareil à tarte	Dans un bol, battez les œufs avec la crème et le lait. Ajoutez le fromage râpé et mélangez bien. Incorporez les crevettes à cette préparation.
36	3	Cuire la tarte	Déroulez la pâte brisée dans un moule à tarte. Versez la préparation sur le fond de pâte, puis enfournez à 180°C pendant environ 30 minutes, jusqu’à ce que la tarte soit dorée.
30	1	Cuire les légumes et la viande	Dans une poêle, faites chauffer l’huile d’olive. Ajoutez l’oignon et l’ail hachés ainsi que le poivron en dés et faites revenir quelques minutes. Incorporez la viande hachée et laissez-la cuire jusqu’à ce qu’elle soit dorée. Assaisonnez avec le sel, le poivre, le paprika et le cumin.
30	2	Ajouter les légumes en conserve	Ajoutez le maïs et les haricots rouges égouttés ainsi que la sauce tomate. Mélangez bien et laissez mijoter quelques minutes pour que les saveurs se mélangent.
30	3	Servir	Servez chaud, seul ou avec des tortillas ou du riz.
31	1	Préparer les morilles et l’échalote	Dans une poêle, faites chauffer le beurre et l’huile d’olive. Ajoutez l’échalote hachée et faites-la revenir quelques minutes. Ajoutez les morilles et laissez cuire 3 à 4 minutes.
31	2	Cuire les tournedos	Dans une autre poêle, saisissez les tournedos quelques minutes de chaque côté selon la cuisson désirée. Salez et poivrez.
31	3	Préparer la sauce et servir	Versez la crème fraîche dans la poêle avec les morilles, mélangez et laissez réduire légèrement. Servez les tournedos nappés de cette sauce.
37	1	Préparer la purée de betterave	Épluchez et coupez les betteraves en morceaux, puis mixez-les avec le fromage frais et la crème. Assaisonnez avec du sel et du poivre.
37	2	Monter les verrines	Dans des verrines, déposez la purée de betterave en couches égales.
37	3	Décorer et servir	Parsemez de ciboulette ciselée sur le dessus et servez frais.
34	1	Préparer le saumon	Assaisonnez les pavés de saumon avec du sel et du poivre, puis arrosez-les de jus de citron.
34	2	Griller le saumon	Faites chauffer l’huile d’olive dans une poêle et faites cuire le saumon 6 à 8 minutes de chaque côté jusqu’à ce qu’il soit doré.
34	3	Décorer et servir	Disposez le saumon sur les assiettes et parsemez de persil ciselé. Servez immédiatement.
61	1	Préparer les légumes et le poulet	Épluchez et coupez les carottes, les pommes de terre et l’oignon. Nettoyez les poireaux et coupez-les en rondelles. Coupez le poulet en morceaux.
61	2	Cuisson	Dans une grande casserole, faites revenir l’oignon et l’ail émincés dans un peu d’huile pendant 2 minutes. Ajoutez les morceaux de poulet et faites-les légèrement dorer. Versez le bouillon, puis ajoutez tous les légumes. Laissez mijoter 30 minutes jusqu’à ce que les légumes et le poulet soient tendres.
61	3	Assaisonnement et finition	Salez et poivrez la soupe. Goûtez et rectifiez l’assaisonnement si nécessaire. Parsemez de persil frais ciselé avant de servir.
66	1	Préparer les légumes et les lardons	Épluchez et coupez les pommes de terre et les carottes en petits dés. Émincez l’oignon et l’ail.
66	2	Cuisson	Dans une grande casserole, faites revenir les lardons sans matière grasse jusqu’à ce qu’ils soient dorés. Ajoutez l’oignon et l’ail, faites revenir 2 minutes. Versez le bouillon et ajoutez les légumes. Laissez mijoter 25 à 30 minutes jusqu’à ce que les légumes soient tendres.
66	3	Finition et assaisonnement	Salez et poivrez selon votre goût. Servez chaud, parsemez de persil frais ciselé.
50	1	Préparer le sirop	Faites chauffer l’eau avec le sucre jusqu’à dissolution complète. Laissez tiédir.
50	2	Mixer les framboises et le basilic	Mixez les framboises avec le jus de citron et les feuilles de basilic. Passez au tamis pour enlever les graines et obtenir un coulis lisse.
50	3	Mélanger avec le sirop	Incorporez le coulis dans le sirop tiède et mélangez bien. Laissez refroidir.
50	4	Congélation	Versez le mélange dans un plat peu profond et placez-le au congélateur. Toutes les 30 minutes, grattez avec une fourchette pour obtenir la texture granitée. Répétez jusqu’à ce que le granité soit complètement pris.
50	5	Service	Servez dans des verres ou coupelles, éventuellement décoré d’une feuille de basilic.
75	1	Préparer l’appareil à flan	Faites chauffer le lait avec la gousse de vanille fendue. Dans un bol, mélangez les œufs, le sucre et la maïzena. Versez le lait chaud sur ce mélange tout en fouettant.
75	2	Cuisson	Préchauffez le four à 180°C. Étalez la pâte dans un moule, versez l’appareil à flan dessus. Enfournez pendant 45 à 50 minutes jusqu’à ce que le flan soit pris et doré.
75	3	Refroidissement et service	Laissez tiédir à température ambiante puis réfrigérez au moins 2 heures avant de démouler et servir.
76	1	Préparer les fonds de tarte	Préchauffez le four à 180°C. Étalez les fonds de pâte sablée dans des moules à tartelette et faites cuire à blanc 15 à 20 minutes jusqu’à ce qu’ils soient dorés. Laissez refroidir.
76	2	Garnir de crème	Remplissez chaque tartelette avec la crème pâtissière froide, lissez la surface avec une spatule.
76	3	Décorer avec les fruits	Disposez harmonieusement les fruits frais sur la crème. Saupoudrez légèrement de sucre glace avant de servir.
65	1	Préparer le bouillon parfumé	Faites chauffer le bouillon de poulet avec le lait de coco. Ajoutez la citronnelle écrasée, le galanga tranché, les feuilles de kaffir et le piment. Laissez frémir 10 minutes pour que les saveurs se diffusent.
65	2	Cuire les crevettes et les champignons	Ajoutez les champignons émincés puis les crevettes. Laissez cuire 5 minutes jusqu’à ce que les crevettes soient roses et les champignons tendres.
65	3	Assaisonner la soupe	Retirez la citronnelle, le galanga et les feuilles de kaffir. Ajoutez le jus et le zeste des citrons verts ainsi que la sauce de poisson. Goûtez et ajustez l’assaisonnement si nécessaire.
65	4	Servir	Servez chaud dans des bols, parsemez de coriandre fraîche ciselée et décorez éventuellement d’un petit piment tranché.
62	1	Faire revenir les légumes et la viande	Faites chauffer l’huile d’olive dans une grande casserole. Ajoutez l’oignon, l’ail, les carottes et le céleri coupés en dés. Faites revenir quelques minutes. Ajoutez ensuite le bœuf haché et faites-le dorer.
62	2	Ajouter le bouillon et les tomates	Versez le bouillon de bœuf et les tomates concassées. Assaisonnez avec les herbes, le sel et le poivre. Portez à ébullition, puis laissez mijoter à feu doux pendant 40 minutes.
62	3	Cuire les pâtes	Ajoutez les pâtes dans la soupe et laissez cuire 8 à 10 minutes jusqu’à ce qu’elles soient tendres.
62	4	Servir	Servez la soupe chaude, éventuellement parsemée de parmesan râpé ou de basilic frais.
83	1	Préparer les ingrédients	Coupe la viande en gros cubes. Épluche et coupe les légumes. Émince l’oignon, l’ail et le poireau.
83	2	Faire revenir la viande	Dans une grande cocotte, fais chauffer l’huile et fais revenir les morceaux de bœuf jusqu’à ce qu’ils soient bien dorés. Ajoute l’oignon et l’ail, laisse revenir 2 minutes.
83	3	Ajouter les légumes	Ajoute tous les légumes coupés, le bouquet garni et le piment entier. Mélange bien.
83	4	Cuisson lente	Verse l’eau, sale et poivre. Couvre et laisse mijoter à feu doux pendant environ 2 heures, jusqu’à ce que la viande soit fondante et les légumes bien cuits.
83	5	Finalisation	Retire le piment et le bouquet garni. Ajuste l’assaisonnement et ajoute un filet de jus de citron vert avant de servir.
85	1	Préparer les ingrédients	Épluche et coupe tous les légumes en morceaux. Coupe la viande en cubes.
85	2	Faire revenir la viande	Dans une grande cocotte, fais chauffer l’huile d’olive et fais revenir les morceaux de bœuf jusqu’à ce qu’ils soient bien dorés. Ajoute l’oignon haché et l’ail écrasé, puis fais revenir 2 minutes.
85	3	Ajouter les légumes	Ajoute les carottes, le poireau, le céleri, le navet, la tomate et les pommes de terre. Mélange bien pour enrober le tout des sucs de cuisson.
85	4	Cuisson lente	Verse le bouillon ou l’eau, ajoute le bouquet garni, sale et poivre. Couvre et laisse mijoter à feu doux pendant environ 2 heures, jusqu’à ce que la viande soit tendre et les légumes fondants.
78	1	Préparez la viande	Assaisonnez le bœuf avec un peu de sauce soja et d’huile de sésame. Faites-le griller à feu vif pendant 2 à 3 minutes de chaque côté pour obtenir une belle coloration. Laissez reposer quelques instants, puis découpez-le en fines lamelles.
78	2	Préparez le bouillon miso	Dans une casserole, faites chauffer le bouillon (ou le dashi). Hors du feu, diluez la pâte miso dans une petite quantité de bouillon chaud, puis reversez le tout dans la casserole. Ajoutez la sauce soja et laissez mijoter à feu doux sans faire bouillir, afin de préserver les arômes du miso.
78	3	Faites cuire les légumes et les nouilles	Ajoutez la carotte, les champignons et le tofu dans le bouillon. Laissez cuire doucement pendant environ 10 minutes.\nDans une autre casserole, faites cuire les nouilles soba ou udon selon les indications du paquet, puis égouttez-les.
78	4	Assemblez la soupe	Disposez les nouilles dans des bols individuels, puis versez le bouillon chaud avec les légumes et le tofu. Déposez ensuite les lamelles de bœuf grillé sur le dessus.
78	5	Terminez la présentation	Parsemez d’oignons nouveaux finement émincés, ajoutez quelques feuilles d’épinards et, si vous le souhaitez, saupoudrez de graines de sésame grillées avant de servir.
79	1	Préparez le bouillon	Faites griller l’oignon et le gingembre (coupés en deux) directement à la flamme ou dans une poêle sans matière grasse jusqu’à ce qu’ils soient légèrement noircis. Cela apportera de la profondeur au bouillon.\nDans une grande marmite, mettez la viande, les épices (badiane, cannelle, clous de girofle, coriandre), l’oignon et le gingembre. Couvrez d’eau froide, portez à ébullition, puis écumez soigneusement.
79	2	Laissez mijoter doucement	Réduisez le feu et laissez mijoter pendant environ 2 heures 30. Ajoutez le nuoc-mâm et le sucre roux à mi-cuisson. Le bouillon doit être clair et parfumé.
79	3	Préparez les garnitures	Pendant que le bouillon cuit, faites tremper les nouilles de riz dans de l’eau chaude pendant environ 10 minutes, puis égouttez-les.\nCoupez finement le bœuf cuit et préparez les herbes fraîches, le citron vert et les condiments pour le service.
79	4	Assemblez la soupe	Répartissez les nouilles de riz dans des bols. Ajoutez les tranches de bœuf cuit et versez le bouillon bouillant par-dessus. La chaleur du bouillon réchauffera la viande.
79	5	Terminez la présentation	Ajoutez des oignons nouveaux, des herbes fraîches (menthe, coriandre), un peu de piment selon votre goût et quelques gouttes de citron vert. Servez avec un peu de sauce hoisin à part.
27	1	Préparez les fruits	Lavez soigneusement tous les fruits. Épluchez la banane, le kiwi, la poire et l’orange. Coupez-les en morceaux de taille moyenne. Épépinez la pomme et coupez-la également en dés.
27	2	Assemblez la salade	Dans un grand saladier, ajoutez tous les fruits coupés ainsi que les raisins et les morceaux d’ananas. Mélangez délicatement pour ne pas écraser les fruits les plus tendres.
27	3	Préparez l’assaisonnement	Dans un petit bol, mélangez le miel et le jus de citron jusqu’à obtenir une sauce fluide. Versez-la sur les fruits et mélangez doucement pour bien les enrober.
27	4	Terminez et servez	Placez la salade au réfrigérateur pendant 30 minutes pour la servir bien fraîche. Avant de déguster, ajoutez quelques feuilles de menthe fraîche pour la décoration et un parfum agréable.
\.
COPY public.ingredients (id, nom) FROM stdin;
1	Viande de porc hachée
2	Vermicelles de riz (ou de soja)
3	Champignons noirs séchés
4	Carotte râpée
5	Oignon finement haché
6	Gousses d'ail écrasées
7	Oeuf 
8	Cuillères à soupe de nuoc-mâm
9	Sucre
10	Poivre moulu
11	Pousses de soja
12	Galettes de riz
13	Eau tiède légèrement sucrée
14	Huile de friture
15	de choucroute crue
16	de lard fumé
17	saucisses de Strasbourg
18	saucisses Montbéliard
19	jarret de porc demi-sel
20	pommes de terre
21	oignons
22	escalopes de poulet
23	d’huile d’olive
24	de miel
25	salade verte (mâche, roquette, laitue...)
26	tomates cerises
27	concombre
28	d’huile d’olive\r\n
29	viande hachée (bœuf 15% MG de préférence)
30	pains burger (briochés de préférence)\n\n
31	fromage cheddar
32	salade verte
33	tomate
34	oignon rouge
35	mayonnaise ou de sauce burger
36	ketchup
37	pâtes (spaghetti, penne ou tagliatelles)
38	tomates bien mûres
39	gousse d’ail
40	d huile d’olive
41	parmesan râpé
42	bœuf (faux-filet ou rumsteck, finement tranché)
43	nouilles de riz
44	bouillon de bœuf
45	oignon émincé
46	d’ail hachée
47	gingembre (2 cm), coupé en fines lamelles
48	badiane (anis étoilé)
49	cannelle
50	sauce nuoc-mâm (ou sauce soja)
51	sucre
52	bœuf à braiser (paleron, gîte, macreuse) 
53	carottes coupées en rondelles
54	pommes de terre coupées en morceaux
55	oignon 
56	gousses d’ail hachées
57	concentré de tomate
58	1 bouillon de bœuf
176	crème liquide entière
177	sachet de sucre vanillé
178	jaunes d'œufs
179	framboises (fraîches ou surgelées)
180	sucre glace
181	cuillère à soupe de jus de citron
182	biscuits sablés écrasés
183	chocolat noir pâtissier
184	chocolat au lait
185	beurre demi-sel
186	crème liquide
187	biscuits sablés émiettés
188	biscuits digestifs ou sablés
189	beurre fondu
190	fromage frais (type Philadelphia)
191	œufs
192	citron (jus et zeste)
193	farine
194	chocolat noir
195	pâte de noisette
196	noisettes concassées
197	feuille de laurier
198	semoule de couscous
199	viande (poulet ou agneau)
200	carottes
201	courgettes
202	oignon
203	boîte de pois chiches (200 g)
204	huile d’olive
205	Sel, poivre, ras-el-hanout
206	lait
207	cacao en poudre
208	maïzena
209	extrait de vanille
210	bananes mûres
211	biscuits secs
212	beurre
213	noix ou amandes (optionnel)
214	cacao
215	fruits rouges mélangés
216	yaourt nature
217	fruits frais
218	miel
219	graines de chia (optionnel)
220	chocolat
221	fruits rouges
222	lasagnes
223	viande hachée
224	sauce tomate
225	fromage râpé
226	mascarpone
227	crabe
228	riz
229	huile
230	moules
231	ail
232	poulet
233	crevettes
234	petits pois
235	poivron
236	tomates
237	olive
238	bouillon
239	pâte à pizza
240	mozzarella
241	basilic
242	poulet entier
243	citron
244	Herbes de Provence
245	filets de poulet
246	safran
247	bouillon de volaille
248	queues de langouste
249	jus de citron
250	ail hachée
251	paprika
252	galettes de riz
253	vermicelles de riz
254	crevettes cuites
255	carotte
256	feuilles de laitue
257	menthe fraîche
258	coriandre
259	Sauce pour nems ou sauce soja
260	feuilles de brick
261	bœuf haché
262	carotte1
263	curry
264	fromage râpé (gruyère ou comté)
265	riz à sushi
266	eau
267	vinaigre de riz
268	sel
269	saumon frais (qualité sashimi)
270	avocat mûr
271	feuilles de nori
272	tortillas de blé
273	poivron rouge
274	cumin
275	Sauce blanche ou sauce piquante
276	pâte brisée
277	crevettes décortiquées
278	crème fraîche
279	persil haché
280	maïs en conserve
281	haricots rouges en conserve
282	tournedos de bœuf
283	morilles fraîches ou réhydratées
284	échalote
285	betteraves cuites
286	fromage frais
287	ciboulette
288	pavés de saumon
289	persil
290	blanc de poulet
291	poireaux
292	oignon\n\n
293	lardons fumés
294	bouillon de volaille ou de légumes
295	poivre
296	framboises
297	basilic frais
298	gousse de vanille
299	pâte sablée
300	crème pâtissière
301	fruits frais (fraises, kiwis, framboises…)
302	lait de coco
303	bouillon de poulet
304	citrons verts
305	citronnelle
306	kaffir
307	galanga
308	piment rouge
309	champignons de Paris
310	sauce de poisson
311	branches de céleri
312	pâtes petites (type coquillettes)
313	tomates concassées
314	herbes de Provence
315	bœuf à mijoter (paleron, jarret ou gîte)
316	poireau
317	giraumon (ou potiron)
318	patate douce
319	navet
320	piment antillais entier
321	bouquet garni (thym, persil, laurier)
322	citron vert
323	bœuf à mijoter (macreuse, jarret ou paleron)
324	branche de céleri
325	bouquet garni (thym, laurier, persil)
326	eau ou de bouillon de bœuf
327	bœuf (faux-filet ou entrecôte)
328	bouillon de bœuf ou de dashi
329	pâte miso (blanche ou rouge)
330	sauce soja
331	huile de sésame
332	champignons shiitake émincés
333	tofu ferme coupé en dés
334	oignons nouveaux
335	carotte coupée en fins bâtonnets
336	nouilles soba ou udon\n\n
337	épinards frais
338	jarret de bœuf
339	plat de côte ou de poitrine de bœuf
340	gingembre frais
341	badiane
342	girofle
343	graines de coriandre
344	sauce nuoc-mâm
345	sucre roux
346	nouilles de riz plates
347	oignon nouveau
348	coriandre fraîche
349	banane
350	pomme
351	poire
352	kiwi
353	orange
354	raisins
355	ananas frais
\.
COPY public.ingredients_recettes (id_ingredient, id_recette, quantite) FROM stdin;
349	27	1
350	27	1
351	27	1
352	27	1
353	27	1
354	27	100 g de
355	27	100 g d’
218	27	2 cuillères à soupe de
249	27	1 cuillère à soupe de
257	27	Quelques feuilles de
176	58	500 ml de
51	58	100 g de
1	1	300g
2	1	50g
3	1	10
4	1	1
5	1	1
6	1	2
7	1	1
8	1	2
9	1	1 cuillère à soupe
10	1	1 cuillère à café
11	1	1 poignée
12	1	20 g
13	1	1 bol
14	1	
177	58	1
178	58	3
179	58	250 g de
180	58	50 g de
181	58	1
182	58	100 g de
183	43	200 g de
184	43	100 g de
51	43	120 g de
185	43	40 g de
186	43	10 cl de
187	43	50 g de
188	45	200 g de
189	45	80 g de
190	45	400 g de
51	45	100 g de
191	45	2
192	45	1
193	45	1 cuillère à soupe de
194	44	150 g de
195	44	100 g de
186	44	50 ml de
196	44	30 g de
15	18	1 kg
16	18	250 g
19	18	1
21	18	2
20	18	4
17	18	4
18	18	2
197	18	1
198	53	300 g de
199	53	500 g de
200	53	2
201	53	2
202	53	1
203	53	1
204	53	1 cuillère à soupe d’
205	53	1
206	46	500 ml de
51	46	100 g de
207	46	50 g de
208	46	2 cuillères à soupe de
209	46	1 cuillère à café d’
206	42	500 ml de
210	42	2
51	42	50 g de
208	42	2 cuillères à soupe de
209	42	1 cuillère à café d’
211	48	200 g de
212	48	100 g de
51	48	100 g de
213	48	50 g de
214	48	2 cuillères à soupe de
215	41	200 g de
51	41	100 g de
186	41	200 ml de
209	41	1 cuillère à café d’
216	47	150 g de
217	47	100 g de
218	47	1 cuillère à soupe de
219	47	1 cuillère à café de
27	19	½
24	19	1 c. à café
23	19	2 c. à soupe
22	19	2
32	19	1
25	19	100 g de
26	19	10
220	51	200 g de
212	51	100 g de
51	51	100 g de
191	51	3
193	51	50 g de
193	49	200 g de
51	49	100 g de
191	49	3
212	49	100 g de
221	49	150 g de
31	20	2 tranches de
36	20	1 c. à soupe de
35	20	2 c. à soupe de
34	20	½
30	20	2
32	20	2 feuilles de
33	20	1
29	20	250 g de
222	35	9 feuilles de
223	35	400 g de
224	35	500 ml de
225	35	200 g de
202	35	1
226	52	250 g de
191	52	2
51	52	50 g de
227	40	500 g de
228	40	200 g de
202	40	1
33	40	1
39	40	1
229	40	1 c. à soupe d’
230	55	1 kg de
20	55	500 g de
202	55	1
231	55	1 gousse d’
212	55	1 c. à soupe de
228	54	250 g de
232	54	200 g de
233	54	150 g de
234	54	100 g de
235	54	1
202	54	1
236	54	2
237	54	1 c. à soupe d’huile d’
238	54	500 ml de
40	21	2 c. à soupe
39	21	1
41	21	30 g de
37	21	200 g de
38	21	2
239	23	1
240	23	100 g de
236	23	2
204	23	1 c. à soupe d’
241	23	Quelques feuilles de
242	33	1
212	33	50 g de
243	33	1
231	33	2 gousses d’
244	33	1
245	24	2
228	24	150 g de
202	24	1
231	24	1 gousse d’
246	24	1 c. à café de
247	24	400 ml de
237	24	2 c. à soupe d’huile d’
248	39	2
189	39	2 c. à soupe de
249	39	1 c. à soupe de
250	39	1 gousse d’
251	39	1 pincée de
58	25	400 ml de
52	25	600 g de
53	25	3
57	25	1 c. à soupe de
56	25	2
55	25	1
54	25	2
252	26	8
253	26	100 g de
254	26	100 g de
255	26	1
27	26	1/2
256	26	4
257	26	Quelques feuilles de
258	26	Quelques feuilles d
259	26	1
260	28	10
261	28	250 g de
202	28	1
231	28	1 gousse d’
262	28	1
263	28	1 cuillère à soupe de
204	28	1 cuillère à soupe d’
212	38	40 g de
193	38	40 g de
206	38	30 cl de
191	38	4
264	38	100 g de
48	22	1 étoile de
42	22	150 g de
44	22	1 L de
49	22	1 bâton de
46	22	1 gousse
47	22	1 morceau de
43	22	150 g de
45	22	1 petit
50	22	1 c. à soupe de
51	22	1 c. à café de
265	32	300 g de
266	32	350 ml d’
267	32	3 c. à soupe de
51	32	2 c. à soupe de
268	32	1 c. à café de
269	32	200 g de
270	32	1
271	32	4
272	29	4
261	29	300 g de
202	29	1
273	29	1
33	29	1
225	29	100 g de
204	29	1 c. à soupe d’
251	29	1 c. à café de
274	29	1 c. à café de
275	29	1
276	36	1
277	36	300 g de
191	36	2
278	36	20 cl de
206	36	10 cl de
202	36	1
231	36	1 gousse d’
204	36	1 c. à soupe d’
225	36	50 g de
279	36	Un peu de
223	30	200 g de
202	30	1
235	30	1
231	30	1 gousse d’
280	30	100 g de
281	30	100 g de
224	30	2 c. à soupe de
204	30	1 c. à soupe d’
251	30	1 c. à café de
274	30	1 c. à café de
282	31	2
283	31	50 g de
284	31	1
278	31	20 cl de
212	31	1 c. à soupe de
204	31	1 c. à soupe d’
285	37	2
286	37	100 g de
278	37	1 c. à soupe de
287	37	Quelques feuilles de
288	34	2
243	34	1
204	34	1 c. à soupe d’
289	34	Quelques brins de
290	61	400 g de
200	61	2
291	61	2
20	61	2
292	61	1
231	61	1 gousse d’
247	61	1,5 L de
293	66	150 g de
20	66	3
200	66	2
202	66	1
231	66	1 gousse d’
294	66	1,2 L de
268	66	1 c. à café de
295	66	½ c. à café de
296	50	300 g de
51	50	100 g de
266	50	500 ml d’
297	50	Quelques feuilles de
249	50	1 cuillère à soupe de
276	75	1
206	75	1 litre de
51	75	120 g de
298	75	1
191	75	6
208	75	100 g de
299	76	4 fonds de
300	76	250 g de
301	76	200 g de
180	76	20 g de
277	65	400 g de
302	65	400 ml de
303	65	500 ml de
304	65	2
305	65	2 tiges de
306	65	2 feuilles de
307	65	2 cm de
308	65	1 petit
309	65	150 g de
310	65	1 cuillère à soupe de
261	62	300 g de
44	62	1 litre de
200	62	2
311	62	2
202	62	1
231	62	2 gousses d’
312	62	100 g de
313	62	400 g de
204	62	1 cuillère à soupe d’
314	62	1 cuillère à café d’
315	83	600 g de
200	83	2
316	83	1
202	83	1
231	83	2 gousses d’
317	83	1 morceau de
318	83	1
319	83	1
320	83	1
321	83	1
229	83	1 cuillère à soupe d’
266	83	1,5 litre d’
322	83	1
323	85	600 g de
200	85	2
20	85	2
316	85	1
324	85	1
319	85	1
202	85	1
231	85	2 gousses d’
33	85	2
325	85	1
326	85	1,5 litre d
204	85	1 cuillère à soupe d’
327	78	400 g de
328	78	1 litre de
329	78	2 cuillères à soupe de
330	78	1 cuillère à soupe de
331	78	1 cuillère à café d’
332	78	100 g de
333	78	tofu ferme coupé en dés
334	78	2
335	78	1
336	78	100 g de
337	78	Quelques feuilles d’
338	79	500 g de
339	79	250 g de
202	79	1
340	79	1 morceau de
341	79	2 étoiles de
49	79	1 bâton de
342	79	3 clous de
343	79	1 cuillère à soupe de
266	79	2 litres d’
344	79	2 cuillères à soupe de
345	79	1 cuillère à soupe de
346	79	250 g de
347	79	1
348	79	1 bouquet de
322	79	1
308	79	1
257	79	Quelques feuilles de
\.
COPY public.recettes (id, nom, temps_preparation, temps_cuisson, difficulte, photo, id_utilisateur) FROM stdin;
61	Soupe de poulet aux légumes	00:15:00	00:40:00	2	/images/recettes/30fc4758-7639-4fcf-84aa-ac921908c60b.png	\N
40	Matété de crabe	00:20:00	00:30:00	2	/images/recettes/crabeAlantillaise.jpg	5
53	Couscous	00:20:00	01:00:00	2	/images/recettes/coucous.jpg	2
41	Dessert d'été	00:10:00	00:01:00	1	/images/recettes/summerDesserts.jpg	5
65	Soupe thaïlandaise aux crevettes	00:15:00	00:20:00	3	/images/recettes/cd8ddd6a-542c-4011-a66b-4c9a08c35a54.png	\N
50	Granité de framboises au basilic	00:15:00	00:00:00	1	/images/recettes/40a169b0-d0b3-4dd6-b949-fb5f3f6ac0e3.png	4
75	Flan parisien	00:20:00	00:50:00	2	/images/recettes/fb5709af-517c-4ce1-a797-b946b5ab4204.png	\N
27	Salade Tutti Frutti	00:20:00	00:00:00	1	/images/recettes/28e6cfcf-2cda-4644-a6c8-75e3647cf606.png	4
49	Gateau aux fruits rouges	00:10:00	00:25:00	2	/images/recettes/gateauFruits.jpg	4
46	Crème au chocolat	00:10:00	00:10:00	1	/images/recettes/cremeChoco.jpeg	6
1	Nems au porc	02:30:00	00:20:00	2	/images/recettes/nems.jpg	1
35	Lasagnes de bœuf	00:15:00	00:40:00	3	/images/recettes/Lasagne.jpg	2
21	Pâtes à l’italienne	00:05:00	00:15:00	2	/images/recettes/pateItalienne.png	5
54	Paëlla	00:15:00	00:25:00	3	/images/recettes/PAELLA.jpeg	2
58	Buche glacée	00:30:00	00:10:00	2	/images/recettes/bucheGlace.png	\N
24	Poulet Riz Safrané	00:15:00	00:40:00	2	/images/recettes/pouletRizSafrané.jpg	4
23	Pizza Margherita	00:10:00	00:15:00	2	/images/recettes/pizzaMargarita.jpg	5
33	Poulet farci aux herbes	00:15:00	01:00:00	3	/images/recettes/poluetFarci.png	2
22	Soupe Pho	00:30:00	01:30:00	3	/images/recettes/pho.jpg	5
39	Queue de langouste grillée	00:15:00	00:10:00	2	/images/recettes/langouste.png	5
26	Rouleaux de Printemps	00:30:00	00:05:00	2	/images/recettes/rouleauxDePringtemps.jpg	4
28	Samossas de Bœuf	00:25:00	00:10:00	2	/images/recettes/samousasBoeuf.jpg	3
38	Souflé au fromage	00:20:00	00:30:00	2	/images/recettes/souffle.png	3
31	Tournedos aux Morilles	00:10:00	00:15:00	3	/images/recettes/tournedosMorilles.jpg	3
43	Carré de chocolat caramel	00:20:00	00:10:00	1	/images/recettes/carresChocolat.jpeg	5
37	Verrine de betterave	00:10:00	00:00:00	1	/images/recettes/verrinesBetterave.jpg	3
29	Tacos mi amor	00:25:00	00:15:00	2	/images/recettes/tacos.png	3
36	Tarte aux crevettes	00:20:00	00:30:00	2	/images/recettes/tarteCrevette.png	3
52	Mascarpone	00:10:00	00:00:00	1	/images/recettes/mascarpone.jpg	4
30	Tex Mex	00:15:00	00:20:00	2	/images/recettes/texmex.png	3
34	Saumon grillé au citron	00:05:00	00:15:00	1	/images/recettes/saumon.jpg	2
32	Sushi de Saumon à l'avocat	00:40:00	00:15:00	3	/images/recettes/Sushi.jpg	2
25	Ragoût de bœuf	00:20:00	02:00:00	2	/images/recettes/ragoutDeBoeuf.jpg	4
55	Moules frite	00:15:00	00:20:00	2	/images/recettes/moulesFrites.jpg	2
47	Dessert minceur	00:05:00	00:00:00	1	/images/recettes/dessertsMinceur.jpg	6
48	Dessert canadien	00:10:00	00:20:00	1	/images/recettes/dessertCanadien.jpg	6
45	Cheescake au citron	00:15:00	00:40:00	1	/images/recettes/cheesecakeCitron.jpg	6
18	Choucroute Garnie	00:15:00	01:30:00	2	/images/recettes/choucroute.png	6
44	Chocolat Hazelnut	00:10:00	00:05:00	1	/images/recettes/chocolateHazelnut.jpg	5
42	Crème dessert à la banane	00:10:00	00:10:00	1	/images/recettes/cremeBananes.jpg	5
51	Gateau au chocolat	00:10:00	00:25:00	2	/images/recettes/gateauAuChocolat.jpg	4
20	Hamburger Maison	00:10:00	00:15:00	2	/images/recettes/hamburger.png	6
19	Filet de poulet sur lit de salade	00:10:00	00:15:00	2	/images/recettes/FiletsPoulet.png	6
66	Soupe paysanne aux lardons	00:10:00	00:35:00	2	/images/recettes/8f4920b9-1627-4328-9830-9be636348891.png	\N
76	Tartelette aux fruits	00:25:00	00:20:00	2	/images/recettes/4ad64fe7-1c7a-4eff-bbd7-562436914133.png	\N
62	Soupe italienne au boeuf	00:20:00	01:00:00	2	/images/recettes/3ddf7af9-c1ff-4819-ba46-9c55993239f9.png	\N
78	Soupe miso au bœuf grillé	00:20:00	00:25:00	3	/images/recettes/a5c52f23-2b4f-4186-859e-8973e542dfc5.png	\N
83	Soupe antillaise	00:30:00.127	02:00:00.136	3	/images/recettes/c613e516-ee0e-4d79-8d3b-aee896650031.png	\N
85	Soupe au boeuf et légumes	00:25:00.693	02:00:00.702	2	/images/recettes/7016568c-5bbc-4d7f-8db0-9e4083bf56e1.png	\N
79	soupe vietnamienne	00:30:00	02:30:00	3	/images/recettes/2a1ed09a-8f08-4014-a15c-4a648350c16e.png	\N
\.
COPY public.roles (id, nom) FROM stdin;
1	Admin
2	Membre
\.
COPY public.utilisateurs (id, identifiant, email, pass_word, role_id) FROM stdin;
1	orane111	orane111@live.fr	orane111	1
2	joss	joss@gmail.com	$2a$11$AV9kH1Ktrfms4kEq9qgm1uRHI7MHbMu.BVUYJguDscJXq6WmSz.wi	2
3	fredon	fredon@gmail.com	$2a$11$fthGLwdMj7BQL4QwE8uwOOPnLlkawjzl0eSqWcxD6th0poNvnsXL6	1
4	dorian	dorian@gmail.com	$2a$11$vJXZ1TObU59mkLftxMer5uhDqv85rRxRcqFYBLU1szi2jS24D0zVi	2
5	Aline	aline@gmail.com	$2a$11$9sxO2gkXhHPtVUw5LNGjmeJoUY1S3S3V.enUWCS04BItZq8jzQ9dO	2
6	Claire	claire@gmail.com	$2a$11$6yUZ/L5Tx4eeEa.FA67JMevliJaoeYXaI4vd0CMkDGL2GQMp5LxD.	2
7	soraya	soraya@gmail.com	$2a$11$WOWzx1gs37k7sxe0ZNvxc.JbwgTCmnsV1MxB2aCn65I6g/v8cFefO	2
\.
SELECT pg_catalog.setval('public.categories_id_seq', 43, true);
SELECT pg_catalog.setval('public.ingredients_id_seq', 355, true);
SELECT pg_catalog.setval('public.recettes_id_seq', 89, true);
SELECT pg_catalog.setval('public.roles_id_seq', 1, false);
SELECT pg_catalog.setval('public.utilisateurs_id_seq', 7, true);ALTER TABLE ONLY public.avis
    ADD CONSTRAINT avis_pkey PRIMARY KEY (id_recette, id_utilisateur);ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_nom_key UNIQUE (nom);ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_pkey PRIMARY KEY (id);ALTER TABLE ONLY public.categories_recettes
    ADD CONSTRAINT categories_recettes_pkey PRIMARY KEY (id_categorie, id_recette);ALTER TABLE ONLY public.etapes
    ADD CONSTRAINT etapes_pkey PRIMARY KEY (id_recette, numero);ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_nom_key UNIQUE (nom);ALTER TABLE ONLY public.ingredients
    ADD CONSTRAINT ingredients_pkey PRIMARY KEY (id);ALTER TABLE ONLY public.ingredients_recettes
    ADD CONSTRAINT ingredients_recettes_pkey PRIMARY KEY (id_ingredient, id_recette);ALTER TABLE ONLY public.recettes
    ADD CONSTRAINT recettes_pkey PRIMARY KEY (id);ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (id);ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_email_key UNIQUE (email);ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_identifiant_key UNIQUE (identifiant);ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_pkey PRIMARY KEY (id);ALTER TABLE ONLY public.avis
    ADD CONSTRAINT fk_avis_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;ALTER TABLE ONLY public.avis
    ADD CONSTRAINT fk_avis_utilisateurs FOREIGN KEY (id_utilisateur) REFERENCES public.utilisateurs(id);ALTER TABLE ONLY public.categories_recettes
    ADD CONSTRAINT fk_categories_recettes_categorie FOREIGN KEY (id_categorie) REFERENCES public.categories(id) ON DELETE CASCADE;ALTER TABLE ONLY public.categories_recettes
    ADD CONSTRAINT fk_categories_recettes_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;ALTER TABLE ONLY public.etapes
    ADD CONSTRAINT fk_etapes_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;ALTER TABLE ONLY public.ingredients_recettes
    ADD CONSTRAINT fk_ingredients_recettes_ingredient FOREIGN KEY (id_ingredient) REFERENCES public.ingredients(id) ON DELETE CASCADE;ALTER TABLE ONLY public.ingredients_recettes
    ADD CONSTRAINT fk_ingredients_recettes_recette FOREIGN KEY (id_recette) REFERENCES public.recettes(id) ON DELETE CASCADE;ALTER TABLE ONLY public.recettes
    ADD CONSTRAINT fk_recettes_utilisateurs FOREIGN KEY (id_utilisateur) REFERENCES public.utilisateurs(id);ALTER TABLE ONLY public.utilisateurs
    ADD CONSTRAINT utilisateurs_role_id_fkey FOREIGN KEY (role_id) REFERENCES public.roles(id);
REVOKE USAGE ON SCHEMA public FROM PUBLIC;
